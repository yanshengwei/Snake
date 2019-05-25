using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
    private static MainUIController _instance;
    public static MainUIController Instance
    {
        get
        {
            return _instance;
        }
    }

    public bool hasBorder = true;
    public bool isPause = false;
    public int score = 0;
    public int length = 0;
    public Text msgText;
    public Text scoreText;
    public Text lengthText;
    public Image pauseImage;
    public Sprite[] pauseSprites;
    public Image bgImage;
    private Color tempColor;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("border", 1) == 0)
        {
            hasBorder = false;
            foreach (Transform t in bgImage.gameObject.transform)
            {
                t.gameObject.GetComponent<Image>().enabled = false;
            }
        }
    }

    void Update()//背景的五个阶段，背景5个颜色（蓝绿黄橙红）
    {
        switch (score / 100)//这里要注意，防止有片段跳过，所以区间最好好连续，，要写出所有的情况，标签设置要合理
        {
            case 0:
            case 1:
            case 2:
                break;
            case 3:
            case 4:
                ColorUtility.TryParseHtmlString("#CCEEFFFF", out tempColor);//out输出字代表输出
                bgImage.color = tempColor;
                msgText.text = "青铜玩家";
                break;
            case 5:
            case 6:
                ColorUtility.TryParseHtmlString("#CCFFDBFF", out tempColor);
                bgImage.color = tempColor;
                msgText.text = "白银玩家";
                break;
            case 7:
            case 8:
                ColorUtility.TryParseHtmlString("#EBFFCCFF", out tempColor);
                bgImage.color = tempColor;
                msgText.text = "黄金玩家" ;
                break;
            case 9:
            case 10:
                ColorUtility.TryParseHtmlString("#FFF3CCFF", out tempColor);
                bgImage.color = tempColor;
                msgText.text = "钻石玩家" ;
                break;
            default:
                ColorUtility.TryParseHtmlString("#FFDACCFF", out tempColor);
                bgImage.color = tempColor;
                msgText.text = "王者";
                break;
        }
    }

    public void UpdateUI(int s = 5, int l = 1)
    {
        score += s;
        length += l;
        scoreText.text = "得分:\n" + score;
        lengthText.text = "长度:\n" + length;
    }

    public void Pause()                                           //UI界面暂停
    {
        isPause = !isPause;
        if (isPause)
        {
            Time.timeScale = 0;                                    //暂停状态时，时间为0
            pauseImage.sprite = pauseSprites[1];//图标设为1
        }
        else
        {
            Time.timeScale = 1;
            pauseImage.sprite = pauseSprites[0];
        }
    }

    public void Home()//回到主页按钮
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
