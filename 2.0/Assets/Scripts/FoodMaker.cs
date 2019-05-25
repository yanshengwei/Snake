using UnityEngine;
using UnityEngine.UI;
//食物制造
public class FoodMaker : MonoBehaviour
{
    private static FoodMaker _instance;//私有成员，通过类名来调用
    public static FoodMaker Instance
    {
        get//获取
        {
            return _instance;
        }
    }
    //食物的生成必须要生成在蛇的路径上，这里限制了食物的生成范围
    public int xlimit = 21;
    public int ylimit = 11;
    public int xoffset = 7;
    public GameObject foodPrefab;
    public GameObject rewardPrefab;
    public Sprite[] foodSprites;//存放所有食物类型的数组
    private Transform foodHolder;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        foodHolder = GameObject.Find("FoodHolder").transform;
        MakeFood(false);
    }

    public void MakeFood(bool isReward)//做食物
    {
        int index = Random.Range(0, foodSprites.Length);//在数组最大的范围内随机生成食物的索引值，即通过索引值来调出显示食物
        GameObject food = Instantiate(foodPrefab);
        food.GetComponent<Image>().sprite = foodSprites[index];//通过获取数组中的位置来显示随机食物
        food.transform.SetParent(foodHolder, false);
        int x = Random.Range(-xlimit + xoffset, xlimit);
        int y = Random.Range(-ylimit, ylimit);
        food.transform.localPosition = new Vector3(x * 30, y * 30, 0);
        if (isReward)
        {
            GameObject reward = Instantiate(rewardPrefab);
            reward.transform.SetParent(foodHolder, false);
            x = Random.Range(-xlimit + xoffset, xlimit);
            y = Random.Range(-ylimit, ylimit);
            reward.transform.localPosition = new Vector3(x * 30, y * 30, 0);
        }
    }
}
