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
    public Text lifeText;
    public Text goldText;

    bool isGameover = false;

    int lifePoint = 100;
    int goldPoint = 500;
    int count = 0;
    int max = 3;

    [SerializeField] int dragonFlag = 0;
    [SerializeField] int witchFlag = 0;
    [SerializeField] int darkLordFlag = 0;
    [SerializeField] int dwarfFlag = 0;
    [SerializeField] int elfFlag = 0;
    [SerializeField] int ogreFlag = 0;
    [SerializeField] int tresureFlag = 0;
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
        goldText = GameObject.Find("GOLD").GetComponent<Text>();
        lifeText = GameObject.Find("LIFE").GetComponent<Text>();
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
        changeLife(0);
        changeGold(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameover && max - count == 0 || lifePoint <= 0)
        {
            charaImage.sprite = sprites[6];
            isDone = true;
            messageManager.SetMessagePanel("GAMEOVER!");
            isGameover = true;
            onEvent = 0;
        }
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

        tresureFlag = 0;
        goblinFlag = 0;
        merchantFlag = 0;
        kingFlag = 0;
        elfFlag = 0;
        ogreFlag = 0;
        dwarfFlag = 0;
        darkLordFlag = 0;
        witchFlag = 0;
        dragonFlag = 0;
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
            int r = Random.Range(0, 110);
            Debug.Log("r:" + r);
            if (r <= 33)
            {
                isDone = true;
                charaImage.sprite = sprites[1];
                messageManager.SetMessagePanel("あなたの目の前に商人が現れた\n対応を選んでください");
                onEvent = 1;
            }
            else if(r > 33 && r <= 66)
            {
                isDone = true;
                charaImage.sprite = sprites[7];
                messageManager.SetMessagePanel("あなたの目の前にゴブリンが現れた\n対応を選んでください");
                onEvent = 2;
            }
            else if (r > 66 && r <= 100)
            {
                isDone = true;
                charaImage.sprite = sprites[8];
                messageManager.SetMessagePanel("あなたの目の前に宝箱が現れた\n対応を選んでください\n1,そのまま開ける\n2,罠の解除を試みる\n3,諦める");
                onEvent = 3;
            }
            else if (r > 100)
            {
                isDone = true;
                charaImage.sprite = sprites[12];
                messageManager.SetMessagePanel("あなたの目の前にオーガが現れた\n対応を選んでください");
                onEvent = 4;
            }
        }

        if (stage.IsHouse(nextPlayerPositionOnTile))
        {
            isDone = true;
            charaImage.sprite = sprites[10];
            messageManager.SetMessagePanel("魔女：\nここまで来るとはやるわね\n");
            onEvent = 9;
        }
        if (stage.IsDungeon(nextPlayerPositionOnTile))
        {
            isDone = true;
            charaImage.sprite = sprites[11];
            messageManager.SetMessagePanel("ドラゴン：\n弱きものよ。この私に何用だ？\n");
            onEvent = 10;
        }
        if (stage.IsForest(nextPlayerPositionOnTile))
        {
            int r = Random.Range(0, 100);
            Debug.Log("r:" + r);
            if (r <= 50)
            {
                isDone = true;
                charaImage.sprite = sprites[2];
                messageManager.SetMessagePanel("エルフ：\n人間が森に何か用かしら？\n");
                onEvent = 6;
            }
            else
            {
                isDone = true;
                charaImage.sprite = sprites[12];
                messageManager.SetMessagePanel("オーガ：\nニンゲン・・・ウマソウ\n");
                onEvent = 4;
            }

            
        }
        if (stage.IsCave(nextPlayerPositionOnTile))
        {
            isDone = true;
            charaImage.sprite = sprites[3];
            messageManager.SetMessagePanel("ドワーフ：\n酒はあるか？\n");
            onEvent = 7;
        }
        if (stage.IsTower(nextPlayerPositionOnTile))
        {
            isDone = true;
            charaImage.sprite = sprites[4];
            messageManager.SetMessagePanel("魔王：\nよくぞ参った！\n人間共を滅ぼすのだ！\nさすれば我が配下に取り立てよう！");
            onEvent = 8;
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
    void meetDragon()
    {

        charaImage.sprite = sprites[11];
        if (dragonFlag == -1)
        {
            isDone = false;
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
        }
        else if (dragonFlag == 0)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("ドラゴン：\n私に頼み事でもするつもりかね？\nその程度の力で・・・！");
                dragonFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("ドラゴン：\n人の身でその態度\n・・・中々に向こう見ずだな");
                dragonFlag = -1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("ドラゴン：\n今、立ち去るのならば見逃そう・・・");
                dragonFlag = -1;
            }
        }
        else
        {
            isDone = false;
            dragonFlag = -1;
        }
    }
    void meetWitch()
    {

        charaImage.sprite = sprites[10];
        if (witchFlag == -1)
        {
            isDone = false;
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
        }
        else if (witchFlag == 0)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("魔女：\nいい態度ね\nそれで用件は？");
                witchFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("魔女：\n生意気な態度ね\nそれでは話にならないわよ");
                witchFlag = -1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("魔女：\nおかえりはあちらよ");
                witchFlag = -1;
            }
        }
        else
        {
            isDone = false;
            witchFlag = -1;
        }
    }
    void meetDarkLord()
    {

        charaImage.sprite = sprites[4];
        if (darkLordFlag == -1)
        {
            isDone = false;
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
        }
        else if (darkLordFlag == 0)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("魔王：\nよく言った！\nそれでこそ地獄の戦士に相応しい！");
                darkLordFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("魔王：\n私に歯向かう気かね？\n考え直したまえ");
                darkLordFlag = -1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("魔王：\nせっかくここまで来て逃げ出すなど\nもったいない事だと思わないかね？");
                darkLordFlag = -1;
            }
        }
        else
        {
            isDone = false;
            darkLordFlag = -1;
        }
    }
    void meetDwarf()
    {

        charaImage.sprite = sprites[3];
        if (dwarfFlag == -1)
        {
            isDone = false;
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
        }
        else if (dwarfFlag == 0)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("ドワーフ：\nまあゆっくりしていけや");
                dwarfFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("ドワーフ：\n喧嘩するのか？イイぜ！");
                dwarfFlag = -1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("ドワーフ：\nおいおい、もう帰るのか？");
                dwarfFlag = -1;
            }
        }
        else
        {
            isDone = false;
            dwarfFlag = -1;
        }
    }
    void meetOgre()
    {

        charaImage.sprite = sprites[12];
        if (ogreFlag == -1)
        {
            isDone = false;
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
        }
        else if (ogreFlag == 0)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("オーガ：\n・・・ウガ！？\n・・・ジュルリ！");
                ogreFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("オーガ：\nオレサマオマエマルカジリ！！！");
                ogreFlag = -1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("オーガ：\nオマエウマソウ・・・！");
                ogreFlag = -1;
            }
        }
        else
        {
            isDone = false;
            ogreFlag = -1;
        }
    }
    void meetElf()
    {

        charaImage.sprite = sprites[2];
        if (elfFlag == -1)
        {
            isDone = false;
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
        }
        else if (elfFlag == 0)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("エルフ：\n人間に迎合する気はないわ");
                elfFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("エルフ：\nこの森はエルフのものよ！\n人間だろうとオーガだろうと\n好きにはさせないわ！");
                elfFlag = -1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("エルフ：\n人間の里に帰りなさい");
                elfFlag = -1;
            }
        }
        else
        {
            isDone = false;
            elfFlag = -1;
        }
    }

    void meetMerchant()
    {
        
        charaImage.sprite = sprites[1];
        Debug.Log("現在の商人フラグ：" + merchantFlag);
        if(merchantFlag == -1)
        {
            isDone = false;
            //charaImage.sprite = sprites[6];
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
            //merchantFlag = 0;
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
                messageManager.SetMessagePanel("商人：\nまあまあ、そう怖い顔せずニ。\n何か買っていってヨ！");
                merchantFlag = 2;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\n帰るのカイ？\nまたネ！");
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
                messageManager.SetMessagePanel("商人：\n掘り出し物ないかって？\n仕方ないなア");
                merchantFlag = 2;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\n買い物はしないのカイ？\nならいい話したげるネ");
                merchantFlag = 6;
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
                messageManager.SetMessagePanel("商人：\nアイヤー！\nそんなにいきり立たないでネ！");
                merchantFlag = 4;
                karmaPoint -= 3;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\nどしたノ？\nもう帰るノ？");
                merchantFlag = 0;
            }
        }
        else if (merchantFlag == 3)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("商人：\n毎度！");
                changeGold(-100);
                merchantFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("商人：\n値切りカイ？参るナ〜\nちょっぴりなら割引できるネ");
                merchantFlag = 5;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\nやっぱりいらなイ？\nなら他のものを買うカイ？");
                merchantFlag = 1;
            }
        }
        else if (merchantFlag == 4)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("商人：\n許してくれるならとっておきの情報あげるヨ！");
                merchantFlag = 6;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("商人：\nヒエ〜！\nお助けエ〜！");
                merchantFlag = 7;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\n許してくれるんだね？\nよかったヨ！");
                merchantFlag = 0;
            }
        }
        else if (merchantFlag == 5)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("商人：\n割引しといたヨ！\nまた来てね！");
                changeGold(-80);
                merchantFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("商人：\n困るヨ〜！");
                merchantFlag = 2;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\nなら耳寄りの情報をサービスしとくネ！");
                merchantFlag = 6;
            }
        }
        else if (merchantFlag == 6)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("商人：\n島の真ん中の塔には魔王が住んでるヨ\n何かよからぬことを考えてるらしいネ");
                merchantFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("商人：\n最近、エルフにきいたんだケド\n森にオーガがよく出るらしいネ\n気をつけるんだヨ");
                merchantFlag = -1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("商人：\n島の東にある小屋には魔法使いがいるらしいヨ\nすごい魔法を使えるらしいネ");
                merchantFlag = -1;
            }
        }
        else if (merchantFlag == 7)
        {
            messageManager.SetMessagePanel("戦闘が始まった");
            merchantFlag = -1;
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
        if (goblinFlag == -1)　//終わり
        {
            isDone = false;
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
            //goblinFlag = 0;
        }
        else if (goblinFlag == 0)　//基本
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
                messageManager.SetMessagePanel("ゴブリン：\nククク・・・！\n臆病者メ！");
                goblinFlag = -1;
                karmaPoint -= 1;
            }
        }

        else if (goblinFlag == 1)　//やや困惑
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("ゴブリン：\nオレト仲良クシタイノカ？");
                goblinFlag = 3;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("ゴブリン：\nム、魔物ガ恐ソロシクナイノカ？\n面白イ！");
                goblinFlag = 4;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("ゴブリン：\n用ガアルノデハナイノカ？");
                goblinFlag = 0;
            }
        }
        else if (goblinFlag == 2)　//怒り
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("ゴブリン：\n挑発シテイルノカ？\nドウイウツモリダ？");
                goblinFlag = 1;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("ゴブリン：\nカカッテコイ！人間！");
                goblinFlag = 5;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("ゴブリン：\n逃ガサンゾ！");
                goblinFlag = 5;
            }
        }

        else if (goblinFlag == 3) //好感触
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("ゴブリン：\nイイダロウ！\n貢物ヲ寄越セ！\nソウスレバ貴様ラノ金貨ヲ代ワリニヤルゾ！");
                goblinFlag = -1;
                karmaPoint -= 1;
                changeGold(50);
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("ゴブリン：\nホウ！\n骨ノアル奴ノヨウダナ！\nデハ、イイ事ヲ教エテヤル！");
                goblinFlag = 4;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("ゴブリン：\n薄気味悪イ奴ダナ\n死ニタクナケレバ失セロ！");
                goblinFlag = 2;
            }
        }
        else if (goblinFlag == 4) //情報を話す
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("ゴブリン：\n島ノ中央ニアル塔ニテ\n魔王様ガ儀式ヲ執リ行ナオウトシテイル");
                goblinFlag = -1;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("ゴブリン：\nオレヲ倒シテモ無駄ダゾ！\nオーガ共ヤ、デーモン共ガノサバルダケダ！");
                goblinFlag = -1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("ゴブリン：\n悪行ヲ続ケレバ\nヤガテ人間共カラ見放サレルダロウ\nダガ、ソウスレバ魔王様モ認メテクダサルハズダ");
                goblinFlag = -1;
            }
        }
        else if (goblinFlag == 5) //戦闘
        {
            messageManager.SetMessagePanel("戦闘が始まった！");
            goblinFlag = -1;
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
            //kingFlag = 0;
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
                kingFlag = 7;
                karmaPoint += 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\nだからどうしたじゃと？\n同じ島に生きる人間として\n力を貸して貰いたいのじゃ！\n褒美をやっても良いぞ！");
                kingFlag = 7;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\n怖気付いたか？\nだがそれでも島を救ってもらいたい！\n褒美は出すぞ！");
                kingFlag = 7;
            }
        }

        else if (kingFlag == 5)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("王：\n謝ってももう遅いわ！\n２度とこの城に入ることを許さん！");
                kingFlag = -1;
                karmaPoint -= 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\nぐぬぬ！言わせておけば！\n生きては帰さんぞ！\n者共であえであえ！");
                kingFlag = 8;
                karmaPoint -= 3;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\n今更怖気ずくとはな！\n身ぐるみをはいで追放してくれるわ！");
                kingFlag = -1;
                karmaPoint -= 1;
            }
        }
        else if (kingFlag == 6)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("王：\n褒美は望みのままじゃ！\n話を聞くが良い！");
                kingFlag = 1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\nわしの望みを叶えれば\n無礼な態度は許してやろう！\nだから話を聞くのじゃ！");
                kingFlag = 1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\n興味がないじゃと？\n貴様、なにが望みだ？");
                kingFlag = 3;
            }
        }
        else if (kingFlag == 7)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("王：\n良かろう！\n支度金を用意させた！\nこれを持って旅に出るが良い！");
                changeGold(1000);
                kingFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("王：\nあくまで褒美にこだわるか！\nならば島を救った証を立てれば\n爵位と1万ゴールドを与えよう！");
                kingFlag = -1;
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("王：\nならば早々に立ち去るが良い！\n臆病者に用はない！");
                kingFlag = -1;
            }
        }
        else if (kingFlag == 8)
        {
            messageManager.SetMessagePanel("戦闘が始まった！");
            kingFlag = -1;
        }
        else
        {
            isDone = false;
            kingFlag = -1;
        }


    }

    void meetTreasure()
    {
        //charaImage.sprite = sprites[8];
        if (tresureFlag == -1)
        {
            isDone = false;
            messageManager.SetMessagePanel("あなたはその場を立ち去った");
            //tresureFlag = 0;
        }
        else if (tresureFlag == 0)
        {
            if (eventManager.button == 1)
            {
                messageManager.SetMessagePanel("あなたは宝箱を開けた！\n100ゴールド手に入れた！");
                changeGold(100);
                charaImage.sprite = sprites[9];
                tresureFlag = -1;
            }
            else if (eventManager.button == 2)
            {
                messageManager.SetMessagePanel("おおっと！石つぶて\nあなたは10ダメージを受けた！");
                tresureFlag = -1;
                changeLife(-10);
            }
            else if (eventManager.button == 3)
            {
                messageManager.SetMessagePanel("あなたは宝箱を開けなかった・・・");
                tresureFlag = -1;
            }
        }
        else
        {
            isDone = false;
            tresureFlag = -1;
        }
    }

    void changeLife(int value)
    {
        lifePoint = lifePoint + value;
        lifeText.text = "LIFE："+ lifePoint.ToString();
    }
    void changeGold(int value)
    {
        goldPoint = goldPoint + value;
        goldText.text = "GOLD：" + goldPoint.ToString();
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
        else if (onEvent == 3)
        {
            meetTreasure();
        }
        else if (onEvent == 4)
        {
            meetOgre();
        }
        else if (onEvent == 5)
        {
            meetKing();
        }
        else if (onEvent == 6)
        {
            meetElf();
        }
        else if (onEvent == 7)
        {
            meetDwarf();
        }
        else if (onEvent == 8)
        {
            meetDarkLord();
        }
        else if (onEvent == 9)
        {
            meetWitch();
        }
        else if (onEvent == 10)
        {
            meetDragon();
        }
        else
        {
            return;
        }
        EventManager.isClick = false;
    }
}
