using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;
using UnityEngine.UI;//协程命名 

public class SnakeHead : MonoBehaviour
{
    public List<Transform> bodyList = new List<Transform>();
    public float velocity = 0.35f;//每隔调用时间调用一次
    public int step;//蛇的步长
    private int x;//移动的增量值
    private int y;//移动的增量值
    private Vector3 headPos;
    private Transform canvas;
    private bool isDie = false;

    public AudioClip eatClip;//吃东西和死亡声音
    public AudioClip dieClip;
    public GameObject dieEffect;
    public GameObject bodyPrefab;
    public Sprite[] bodySprites = new Sprite[2];

    void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
        //通过Resources.Load(string path)方法加载资源，path的书写不需要加Resources/以及文件扩展名
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(PlayerPrefs.GetString("sh", "sh02"));
        bodySprites[0] = Resources.Load<Sprite>(PlayerPrefs.GetString("sb01", "sb0201"));
        bodySprites[1] = Resources.Load<Sprite>(PlayerPrefs.GetString("sb02", "sb0202"));
    }

    void Start()//调用移动函数
    {
        InvokeRepeating("Move", 0, velocity);//持续调用（函数名，等待调用时间，每隔调用时间调用一次）
        x = 0;y = step;//蛇初始运动方向
    }

    void Update()//获取键盘移动信息（WASD）
    {
        if (Input.GetKeyDown(KeyCode.Space) && MainUIController.Instance.isPause == false && isDie == false)//按空格加速
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity - 0.2f);
        }
        if (Input.GetKeyUp(KeyCode.Space) && MainUIController.Instance.isPause == false && isDie == false)//松空格减速
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity);
        }
                                                  
        
        //死亡后不准玩家再操作
        if (Input.GetKey(KeyCode.W) && y != -step && MainUIController.Instance.isPause == false && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);//让蛇头转动 
            x = 0;y = step;
        }
        if (Input.GetKey(KeyCode.S) && y != step && MainUIController.Instance.isPause == false && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
            x = 0; y = -step;
        }
        if (Input.GetKey(KeyCode.A) && x != step && MainUIController.Instance.isPause == false && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
            x = -step; y = 0;
        }
        if (Input.GetKey(KeyCode.D) && x != -step && MainUIController.Instance.isPause == false && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
            x = step; y = 0;
        }
    }

    void Move()                                                  //蛇的移动
    {
        headPos = gameObject.transform.localPosition;                                               //保存下来蛇头移动前的位置
        gameObject.transform.localPosition = new Vector3(headPos.x + x, headPos.y + y, headPos.z);  //蛇头向期望位置移动
        if (bodyList.Count > 0)
        {
            //由于我们是双色蛇身，此方法弃用
            //bodyList.Last().localPosition = headPos;                                              //将蛇尾移动到蛇头移动前的位置
            //bodyList.Insert(0, bodyList.Last());                                                  //将蛇尾在List中的位置更新到最前
            //bodyList.RemoveAt(bodyList.Count - 1);                                                //移除List最末尾的蛇尾引用

            //由于我们是双色蛇身，使用此方法达到显示目的
            for (int i = bodyList.Count - 2; i >= 0; i--)                                           //从后往前开始移动蛇身
            {
                bodyList[i + 1].localPosition = bodyList[i].localPosition;                          //每一个蛇身都移动到它前面一个节点的位置
            }
            bodyList[0].localPosition = headPos;                                                    //第一个蛇身移动到蛇头移动前的位置
        }
    }

    void Grow()//身体吃食物生长出来
    {
        AudioSource.PlayClipAtPoint(eatClip, Vector3.zero);//吃东西声音
        int index = (bodyList.Count % 2 == 0) ? 0 : 1;//用来判断新加的身体是什么颜色，通过奇偶性来判断，之前已经设数组来存放身体的所有可能
        GameObject body = Instantiate(bodyPrefab, new Vector3(2000, 2000, 0), Quaternion.identity);
        body.GetComponent<Image>().sprite = bodySprites[index];
        body.transform.SetParent(canvas, false);
        bodyList.Add(body.transform);//身体长出来一节就要让头知道
    }

    void Die()
    {
        AudioSource.PlayClipAtPoint(dieClip, Vector3.zero);//死亡音效
        CancelInvoke();
        isDie = true;
        Instantiate(dieEffect);
        
        //记录得分成绩
        PlayerPrefs.SetInt("lastl", MainUIController.Instance.length);
        PlayerPrefs.SetInt("lasts", MainUIController.Instance.score);
        //历史最好成绩
        if (PlayerPrefs.GetInt("bests", 0) < MainUIController.Instance.score)
        {
            PlayerPrefs.SetInt("bestl", MainUIController.Instance.length);
            PlayerPrefs.SetInt("bests", MainUIController.Instance.score);
        }
        StartCoroutine(GameOver(1.5f));
    }

    IEnumerator GameOver(float t)
    {
        yield return new WaitForSeconds(t);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            Destroy(collision.gameObject);
            MainUIController.Instance.UpdateUI();
            Grow();
            FoodMaker.Instance.MakeFood((Random.Range(0, 100) < 40) ? true : false);//吃完一个食物再生成一个食物，假如奖励随机生成小于20%，就再生成一个奖励目标
        }
        else if (collision.gameObject.CompareTag("Reward"))//假如吃到奖励
        {
            Destroy(collision.gameObject);//长身体
            MainUIController.Instance.UpdateUI(Random.Range(5, 15) * 10);//吃奖励身体得分随机增加50~150
            Grow();
        }
        else if (collision.gameObject.CompareTag("Body"))                   //当撞到身体时，死亡
        {
            Die();
        }
        else
        {
            if (MainUIController.Instance.hasBorder)
            {
                Die();
            }
            else
            {
                switch (collision.gameObject.name)//自由模式蛇头传送   
                {
                    case "Up":
                        transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y + 30, transform.localPosition.z);
                        break;
                    case "Down":
                        transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y - 30, transform.localPosition.z);
                        break;
                    case "Left":
                        transform.localPosition = new Vector3(-transform.localPosition.x + 180, transform.localPosition.y, transform.localPosition.z);
                        break;
                    case "Right":
                        transform.localPosition = new Vector3(-transform.localPosition.x + 240, transform.localPosition.y, transform.localPosition.z);
                        break;
                }
            }
        }
    }
}
