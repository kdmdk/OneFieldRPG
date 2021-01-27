using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerManager : MonoBehaviour
{
    public StageManager stage = default;
    public GameManager gameManager = default;
    public MessageManager messageManager;
    public Image charaImage;
    public string daysLeft;
    public Text countText;
    public Sprite[] sprites;
    public Text karma;

    int count = 0;
    int max = 100;

    [SerializeField] int goblinFlag = 0;
    [SerializeField] int merchantFlag = 0;
    [SerializeField] int kingFlag = 0; //　王のイベントを管理
    [SerializeField] public int karmaPoint = 0; //　カルマ値（善行、悪行を示す）

    public static Vector2Int currentPlayerPositionOnTile;               // 1.現在の位置を取得
    public static Vector2Int nextPlayerPositionOnTile; // 2.次の位置を取得

    private Animator animator = null;

    bool isDone = false;

    public static int onEvent = 0;

    private EventManager eventManager;

    // Start is called before the first frame update
    void Start()
    {
        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();
        karma = GameObject.Find("KARMA").GetComponent<Text>();
        countText = GameObject.Find("CountDown").GetComponent<Text>();
        daysLeft = GameObject.Find("CountDown").GetComponent<Text>().text;
        charaImage = GameObject.Find("CharaImage").GetComponent<Image>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        stage = GameObject.Find("StageManager").GetComponent<StageManager>();
        animator = GetComponent<Animator>();
        messageManager = GameObject.Find("MessageUI").GetComponent<MessageManager>();

        //countText.text = "残り<b><size=60>" + max + "</size></b>日";
        daysLeft = (max - count).ToString();
        countText.text = "残り<b><size=60>" + daysLeft + "</size></b>日";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //animator.SetInteger("direction", 3);
            MoveTo(DIRECTION.UP);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //animator.SetInteger("direction", 0);
            MoveTo(DIRECTION.DOWN);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //animator.SetInteger("direction", 1);
            MoveTo(DIRECTION.LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //animator.SetInteger("direction", 2);
            MoveTo(DIRECTION.RIGHT);
        }
        if (EventManager.isClick)
        {

            EventBranch();
        }
        karma.text = "KARMA：" + karmaPoint.ToString();
    }
    public void Move(Vector2 position, DIRECTION direction)
    {
        transform.position = position;
        
        //MessageManager.isTextSound = true;
    }
    void MoveTo(DIRECTION direction)
    {
//        Debug.Log(direction);
        currentPlayerPositionOnTile = stage.moveObjPositionOnTile[this.gameObject];               // 1.現在の位置を取得
        nextPlayerPositionOnTile = GetNextPositionOnTile(currentPlayerPositionOnTile, direction); // 2.次の位置を取得

        //Playerの移動先がSEAかMOUNTAINのとき
        if (stage.IsMountain(nextPlayerPositionOnTile) || stage.IsSea(nextPlayerPositionOnTile) || isDone)
        {
            return;//処理をここで終了させる(下の処理を行わない)海か山があるならここで終了
        }
        
        animator.SetInteger("direction", (int)direction);

        if (stage.IsCastle(nextPlayerPositionOnTile))
        {
            isDone = true;
            charaImage.sprite = sprites[0];
            messageManager.SetMessagePanel("あなたはお城に行き王様に会った\n対応を選んでください");
            onEvent = 5;
        }
        if (stage.IsGround(nextPlayerPositionOnTile))
        {
            int r = Random.Range(0, 100);
            Debug.Log("r:" + r);
            if (r >= 50)
            {
                isDone = true;
                charaImage.sprite = sprites[1];
                messageManager.SetMessagePanel("あなたの目の前に商人が現れた\n対応を選んでください");
                onEvent = 1;
            }
            else
            {
                isDone = true;
                charaImage.sprite = sprites[7];
                messageManager.SetMessagePanel("あなたの目の前にゴブリンが現れた\n対応を選んでください");
                onEvent = 2;
            }
            

        }
        if (stage.IsForest(nextPlayerPositionOnTile))
        {
            charaImage.sprite = sprites[2];
            messageManager.SetMessagePanel("エルフ：\n人間が森に何か用かしら？\n");
        }
        if (stage.IsCave(nextPlayerPositionOnTile))
        {
            charaImage.sprite = sprites[3];
            messageManager.SetMessagePanel("ドワーフ：\n酒はあるか？\n");
        }
        if (stage.IsTower(nextPlayerPositionOnTile))
        {
            charaImage.sprite = sprites[4];
            messageManager.SetMessagePanel("魔王：\nよくぞ参った！\n人間共を滅ぼすのだ！\nさすれば我が配下に取り立てよう！");
        }
        if (stage.IsPlayerInitialPosition(nextPlayerPositionOnTile))
        {
            charaImage.sprite = sprites[5];
            messageManager.SetMessagePanel("妖精：\nこの島はあと" + (max - count - 1) + "日で滅びます！\n島を救うもよし、脱出するもよし、\n自ら滅ぼすもよし。\n身の振り方を考えましょう！");
        }
        //stage.UpdateTileTableForPlayer(currentPlayerPositionOnTile, nextPlayerPositionOnTile);
        this.Move(stage.GetScreenPositionFromTileTable(nextPlayerPositionOnTile), direction);              // 3.次の位置にプレイヤーを移動
        stage.moveObjPositionOnTile[this.gameObject] = nextPlayerPositionOnTile;                           // 4.タイル情報も更新
        daysLeft = (max - ++count).ToString();
        countText.text = "残り<b><size=60>" + daysLeft + "</size></b>日";

    }
    Vector2Int GetNextPositionOnTile(Vector2Int currentPosition, DIRECTION direction)
    {
        switch (direction)
        {
            case DIRECTION.UP:
                return currentPosition + Vector2Int.down;
            case DIRECTION.DOWN:
                return currentPosition + Vector2Int.up;
            case DIRECTION.LEFT:
                return currentPosition + Vector2Int.left;
            case DIRECTION.RIGHT:
                return currentPosition + Vector2Int.right;
        }
        return currentPosition;
    }

    void meetMerchant()
    {
        
        charaImage.sprite = sprites[1];

        if(merchantFlag == -1)
        {
            isDone = false;
            //charaImage.sprite = sprites[6];
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
            merchantFlag = 0;
        }
        else if(merchantFlag == 0)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("商人：\nお得な商品入荷したヨ？\n見てってネ！");
                merchantFlag = 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("商人：\nまあまあ、そういきりたたずニ。\n何か買っていってヨ！");
                merchantFlag = 2;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\n帰るのかい？\nまたネ！");
                merchantFlag = -1;
            }
        }
        else if(merchantFlag == 1)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("商人：\n鉄の剣があるヨ？\n買ってくカイ！");
                merchantFlag = 3;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("商人：\n安くしろっテ？\n仕方ないなア");
                merchantFlag = 2;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\n買い物はしないのカイ？");
                merchantFlag = 0;
            }
        }
        else if (merchantFlag == 2)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("商人：\nそれならとっておきネ！\n魔法の剣があるヨ？\n買ってくダロ！");
                merchantFlag = 3;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("商人：\nひええ！\nお助け〜！");
                merchantFlag = 4;
                karmaPoint -= 3;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\n？");
                merchantFlag = 0;
            }
        }
        else if (merchantFlag == 3)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("商人：\n毎度！");
                merchantFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("商人：\n値切りカイ？参るナ〜");
                merchantFlag = 5;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\nやっぱりいらなイ？\nなら他のものを買うカイ？");
                merchantFlag = 1;
            }
        }
        else
        {
            isDone = false;
            merchantFlag = -1;
        }
    }
    void meetGoblin()
    {
        charaImage.sprite = sprites[7];
        if (goblinFlag == -1)
        {
            isDone = false;
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
            goblinFlag = 0;
        }
        else if (goblinFlag == 0)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("ゴブリン：\nナンノ用ダ！");
                goblinFlag = 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("ゴブリン：\nヤルカ！？人間！");
                goblinFlag = 2;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("ゴブリン：\nヒヒヒ・・・！\n臆病者メ！");
                goblinFlag = -1;
                karmaPoint -= 1;
            }
        }

        else if (goblinFlag == 1)
        {
            if (eventManager.button == 1)
            {
                //nオマエモ魔王サマノ下デ働キタイノカ
                messageManager.SetMessagePanel("ゴブリン：\nオレト仲良クシタイノカ？");
                goblinFlag = 3;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("ゴブリン：\n魔物ガ恐ソロシクナイノカ？");
                goblinFlag = 4;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("ゴブリン：\n用ガアルノデハナイノカ？");
                goblinFlag = 0;
            }
        }
        else if (goblinFlag == 2)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("ゴブリン：\n？");
                goblinFlag = 1;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("ゴブリン：\nカカッテコイ！人間！");
                goblinFlag = 2;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("ゴブリン：\n逃ガサンゾ！");
                goblinFlag = -1;
            }
        }
        else
        {
            isDone = false;
            goblinFlag = -1;
        }
    }
    void meetKing()
    {
        
        charaImage.sprite = sprites[0];

        if (kingFlag == -1)
        {
            isDone = false;
            //charaImage.sprite = sprites[6];
            messageManager.SetMessagePanel("あなたは城を立ち去った");
            kingFlag = 1;
        }
        else if (kingFlag == 0)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("王：\nよく来たな勇者よ！\nわしの望みを聞いてくれい！");
                kingFlag = 1;
                karmaPoint += 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\n無礼者め！\n身の程を知れ！！");
                kingFlag = 2;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\n帰るのか？\n残念じゃのう！");
                kingFlag = 3;
            }
        }
        else if (kingFlag == 1)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("王：\n実はこの島はあとわずかで滅びるという予言が出たのじゃ！");
                kingFlag = 4;
                karmaPoint += 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\nなに？！\nまずは褒美の話じゃと？");
                kingFlag = 6;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\nなに？嫌だと申すのか？\n残念じゃのう！");
                kingFlag = 3;
            }
        }
        else if (kingFlag == 2)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("王：\n褒美次第で話は聞くじゃと？\n傲慢な奴じゃな！");
                kingFlag = 6;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\nわ、わしを倒すじゃと？！\n痴れ者が！！");
                kingFlag = 5;
                karmaPoint -= 2;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\nなにをしに来たんじゃ貴様は！？\n去れ！");
                kingFlag = 3;
            }
        }
        else if (kingFlag == 3)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("王：\n話を聞きたいじゃと？\n存外殊勝な奴じゃな");
                kingFlag = 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\nええい、忌々しい、帰れ！");
                kingFlag = -1;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\nまあ良い、話が聞きたかったらまた来るが良い！");
                kingFlag = -1;
            }

        }
        else if (kingFlag == 4)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("王：\n勇者よ！この島を救ってくれい！\n其方がこの島を救った暁には\n望みの褒美を与えよう！");
                kingFlag = 0;
                karmaPoint += 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\nだからどうしたじゃと？\n同じ島に生きる人間として\n力を貸して貰いたいのじゃ！\n褒美をやっても良いぞ！");
                kingFlag = 0;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\n怖気付いたか？\nだがそれでも島を救ってもらいたい！\n褒美は出すぞ！");
                kingFlag = 0;
            }
        }

        else if (kingFlag == 5)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("王：\n謝ってももう遅いわ！\n２度とこの城に入ることを許さん！");
                kingFlag = 0;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\nぐぬぬ！言わせておけば！\n生きては帰さんぞ！\n者共であえであえ！");
                kingFlag = 0;
                karmaPoint -= 3;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\n今更怖気ずくとはな！\n身ぐるみをはいで追放してくれるわ！");
                kingFlag = 0;
                karmaPoint -= 1;
            }
        }
        else if (kingFlag == 6)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("王：\n褒美は望みのままじゃ！\n話を聞くが良い！");
                kingFlag = 4;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\nわしの望みを叶えれば\n無礼な態度は許してやろう！\nだから話を聞くのじゃ！");
                kingFlag = 4;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\n興味がないじゃと？\n貴様、なにが望みだ？");
                kingFlag = 3;
            }
        }


        else
        {
            isDone = false;
            kingFlag = -1;
        }


    }

    public void EventBranch()
    {
        if (onEvent == 1)
        {
            meetMerchant();
        }
        else if (onEvent == 2)
        {
            meetGoblin();
        }
        else if (onEvent == 5)
        {
            meetKing();
        }
        EventManager.isClick = false;
    }
}
