using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Collections;


public class chess_mal : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    heuristic_imformation ai = new heuristic_imformation();
    private TransformAccessArray _transformAccessArray;

    DateTime t;
    string date;

    public Toggle White_tog, Black_tog;

    ArrayList next_loca;
    ArrayList pre_loca;
    GameObject login_information_;
    public Text ai_wait_text, End_Text;

    string AI, PLAYER;
    string[] ai_text = new string[4] { "계산중.", "계산중..", "계산중...", "계산중...." };
    //기본 변수들

    public GameObject blackbackground, Square, Sqaure2;
    bool calc_heuristic = false; //AI계산 시작
    bool wait_ai = false; //text표시

    string[,] chess_pan = new string[8, 8];
    string[,] chess_nemo = new string[8, 8];
    GameObject chess_pan_imge;
    GameObject[] Black_pawn = new GameObject[8];
    GameObject[] Black_knight = new GameObject[2];
    GameObject[] Black_bishop = new GameObject[2];
    GameObject[] Black_rook = new GameObject[2];
    GameObject Black_king;
    GameObject[] Black_queen = new GameObject[9];
    GameObject[] White_pawn = new GameObject[8];
    GameObject[] White_knight = new GameObject[2];
    GameObject[] White_bishop = new GameObject[2];
    GameObject[] White_rook = new GameObject[2];
    GameObject White_king;
    GameObject[] White_queen = new GameObject[9];

    public GameObject AIColor_Black, AIColor_White, PlayerColor_Black, PlayerColor_White;

    private Thread th;

    string player1;
    GameObject nemo;

    int pre_x, pre_y;
    // Start is called before the first frame update
    void Start()
    {
        check_pan_pre = new ArrayList();
        t = DateTime.Now;
        date = t.ToString("yyyy/MM/dd HH:mm:ss");
        ai_wait_text.enabled = false;
        next_loca = new ArrayList();
        pre_loca = new ArrayList();
        login_information_ = GameObject.Find("Login_");
        nemo = GameObject.FindGameObjectWithTag("temp");
        Sqaure2.SetActive(false);
        AIColor_Black.SetActive(false);
        AIColor_White.SetActive(false);
        PlayerColor_Black.SetActive(false);
        PlayerColor_White.SetActive(false);
    }

    public void initPosition()
    {
        chess_pan[0, 7] = "Black_rook0";
        chess_pan[1, 7] = "Black_knight0";
        chess_pan[2, 7] = "Black_bishop0";
        chess_pan[3, 7] = "Black_queen0";
        chess_pan[4, 7] = "Black_king;";
        chess_pan[7, 7] = "Black_rook1";
        chess_pan[6, 7] = "Black_knight1";
        chess_pan[5, 7] = "Black_bishop1";

        chess_pan[0, 0] = "White_rook0";
        chess_pan[1, 0] = "White_knight0";
        chess_pan[2, 0] = "White_bishop0";
        chess_pan[3, 0] = "White_queen0";
        chess_pan[4, 0] = "White_king;";
        chess_pan[7, 0] = "White_rook1";
        chess_pan[6, 0] = "White_knight1";
        chess_pan[5, 0] = "White_bishop1";
        for (int i = 0; i < 8; i++)
        {
            chess_pan[i, 6] = "Black_pawn" + (i + 1).ToString();
            chess_pan[i, 1] = "White_pawn" + (i + 1).ToString();
        }
    }

    public void init_graphic()
    {
        for (int i = 0; i < 8; i++)
        {
            graphic_setting(Black_pawn[i], i, 6);
            graphic_setting(White_pawn[i], i, 1);
        }
        graphic_setting(Black_knight[0], 1, 7);
        graphic_setting(Black_bishop[0], 2, 7);
        graphic_setting(Black_rook[0], 0, 7);
        graphic_setting(Black_knight[1], 6, 7);
        graphic_setting(Black_bishop[1], 5, 7);
        graphic_setting(Black_rook[1], 7, 7);

        graphic_setting(White_knight[0], 1, 0);
        graphic_setting(White_bishop[0], 2, 0);
        graphic_setting(White_rook[0], 0, 0);
        graphic_setting(White_knight[1], 6, 0);
        graphic_setting(White_bishop[1], 5, 0);
        graphic_setting(White_rook[1], 7, 0);

        graphic_setting(Black_king, 4, 7);
        graphic_setting(Black_queen[0], 3, 7);

        graphic_setting(White_king, 4, 0);
        graphic_setting(White_queen[0], 3, 0);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(),
            eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
            return;
        localCursor.x = localCursor.x + 427;
        localCursor.y = localCursor.y + 427;
        double x = (localCursor.x - 8.67616) / 104.80004;
        double y = (localCursor.y - 9.060577) / 104.80004;
        Debug.Log("현재 누른 말 : " + chess_pan[(int)x, (int)y] + " 입니다.");
        if (chess_nemo[(int)x, (int)y].Contains("nemo")) //네모칸 클릭시
        {
            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("temp"); //판위의 네모모양이 있는 개수
            for (int i = 0; i < movePlates.Length; i++)
            {
                if (movePlates[i].name != "MovePlate1")
                {
                    Destroy(movePlates[i]); //네모 칸 다 파괴
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int h = 0; h < 8; h++)
                {
                    if (chess_nemo[h, i].Contains("nemo"))
                    {
                        chess_nemo[h, i] = "0";
                    }
                }
            }
            pre_loca.Add(new Vector2(pre_x, pre_y));
            next_loca.Add(new Vector2((int)x, (int)y));
            calc_heuristic = true;
            Move(chess_pan[(int)pre_x, (int)pre_y], (int)x, (int)y);
            System.Threading.Thread.Sleep(200);
            ai_wait_text.enabled = true;
            wait_ai = true;
        }
        else if ((chess_pan[(int)x, (int)y].Contains("/") == false) && (chess_pan[(int)x, (int)y].Contains(PLAYER))) //다른 말 클릭시
        {
            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("temp"); //판위의 네모모양이 있는 개수
            for (int i = 0; i < movePlates.Length; i++)
            {
                if (movePlates[i].name != "MovePlate1")
                {
                    Destroy(movePlates[i]); //네모 칸 다 파괴
                }
            }
            for (int i = 0; i < 8; i++) { for (int h = 0; h < 8; h++) { if (chess_nemo[h, i].Contains("nemo")) { chess_nemo[h, i] = "0"; } } }
            pre_x = (int)x;
            pre_y = (int)y;
            MovePredict(chess_pan[(int)x, (int)y], (int)x, (int)y);
        }
        else //빈칸 클릭시
        {
            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("temp"); //판위의 네모모양이 있는 개수
            for (int i = 0; i < movePlates.Length; i++)
            {
                if (movePlates[i].name != "MovePlate1")
                {
                    Destroy(movePlates[i]); //네모 칸 다 파괴
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int h = 0; h < 8; h++)
                { if (chess_nemo[h, i].Contains("nemo")) { chess_nemo[h, i] = "0"; } }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (calc_heuristic)
        {
            check_pan_pre.Clear();
            cnttt = 0;
            th = new Thread(Run);
            th.Start();
            System.Threading.Thread.Sleep(500);
            int total = heuristic(4, AI, -99999999, 99999999);
            Debug.Log("휴릭스틱에서 바뀔 값 : " + chess_pan[pre_x, pre_y]);
            pre_loca.Add(new Vector2(pre_x, pre_y));
            next_loca.Add(new Vector2((int)ai.loca.x, (int)ai.loca.y));
            Move(ai.mal, (int)ai.loca.x, (int)ai.loca.y);
            calc_heuristic = false;
            wait_ai = false;
            th.Abort();
            ai_wait_text.enabled = false;
        }
    }

    public void attack(int x, int y, string cur_player) //공격당한 말 삭제
    {
        if (cur_player.Contains("White"))
        {
            string other_mal = chess_pan[x, y];
            Debug.Log("적의 말은 " + other_mal);
            if (other_mal.Contains("pawn"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1)) - 1;
                Destroy(Black_pawn[r]);
            }
            else if (other_mal.Contains("knight"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1));
                Destroy(Black_knight[r]);

            }
            else if (other_mal.Contains("bishop"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1));
                Destroy(Black_bishop[r]);
            }
            else if (other_mal.Contains("rook"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1));
                Destroy(Black_rook[r]);
            }
            else if (other_mal.Contains("king"))
            {
                Destroy(Black_king);
                login_information_.GetComponent<Login_information>().set_upgrade_chess(pre_loca, next_loca, player1, date);
                calc_heuristic = false;
                if (cur_player == PLAYER)
                {
                    blackbackground.SetActive(true);
                    Sqaure2.SetActive(true);
                    End_Text.text = "승리";
                }
                else
                {
                    blackbackground.SetActive(true);
                    Sqaure2.SetActive(true);
                    End_Text.text = "패배";
                }

            }
            else if (other_mal.Contains("queen"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1));
                Destroy(Black_queen[r]);
            }
        }
        else
        {
            string other_mal = chess_pan[x, y];
            if (other_mal.Contains("pawn"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1)) - 1;
                Destroy(White_pawn[r]);
            }
            else if (other_mal.Contains("knight"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1));
                Destroy(White_knight[r]);
            }
            else if (other_mal.Contains("bishop"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1));
                Destroy(White_bishop[r]);

            }
            else if (other_mal.Contains("rook"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1));
                Destroy(White_rook[r]);
            }
            else if (other_mal.Contains("king"))
            {
                Destroy(White_king);
                login_information_.GetComponent<Login_information>().set_upgrade_chess(pre_loca, next_loca, player1, date);
                calc_heuristic = false;
                if (cur_player == PLAYER)
                {
                    blackbackground.SetActive(true);
                    Sqaure2.SetActive(true);
                    End_Text.text = "승리";
                }
                else
                {
                    blackbackground.SetActive(true);
                    Sqaure2.SetActive(true);
                    End_Text.text = "패배";
                }

            }
            else if (other_mal.Contains("queen"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1));
                Destroy(White_queen[r]);
            }
        }
    }
    public void Move(string mal, int x, int y)
    {
        if (chess_pan[x, y] != "/") //공격
        {
            attack(x, y, mal.Substring(0, 5));
        }
        chess_pan[x, y] = chess_pan[pre_x, pre_y];
        chess_pan[pre_x, pre_y] = "/";

        if (mal.Contains("White"))
        {
            if (mal.Contains("pawn"))
            {
                int r = int.Parse(mal.Substring(mal.Length - 1, 1)) - 1;
                graphic_setting(White_pawn[r], x, y);
                if (y == 7)
                {
                    pawn_upgrade(mal, x, y);
                }
            }
            else if (mal.Contains("knight"))
            {
                int r = int.Parse(mal.Substring(mal.Length - 1, 1));
                graphic_setting(White_knight[r], x, y);
            }
            else if (mal.Contains("bishop"))
            {
                int r = int.Parse(mal.Substring(mal.Length - 1, 1));
                graphic_setting(White_bishop[r], x, y);
            }
            else if (mal.Contains("rook"))
            {
                int r = int.Parse(mal.Substring(mal.Length - 1, 1));
                graphic_setting(White_rook[r], x, y);
            }
            else if (mal.Contains("king"))
            {
                graphic_setting(White_king, x, y);
            }
            else if (mal.Contains("queen"))
            {
                int r = int.Parse(mal.Substring(mal.Length - 1, 1));
                graphic_setting(White_queen[r], x, y);
            }
        }
        else
        {
            if (mal.Contains("pawn"))
            {
                int r = int.Parse(mal.Substring(mal.Length - 1, 1)) - 1;
                graphic_setting(Black_pawn[r], x, y);
                if (y == 0)
                {
                    pawn_upgrade(mal, x, y);
                }
            }
            else if (mal.Contains("knight"))
            {
                int r = int.Parse(mal.Substring(mal.Length - 1, 1));
                graphic_setting(Black_knight[r], x, y);
            }
            else if (mal.Contains("bishop"))
            {
                int r = int.Parse(mal.Substring(mal.Length - 1, 1));
                graphic_setting(Black_bishop[r], x, y);
            }
            else if (mal.Contains("rook"))
            {
                int r = int.Parse(mal.Substring(mal.Length - 1, 1));
                graphic_setting(Black_rook[r], x, y);
            }
            else if (mal.Contains("king"))
            {
                graphic_setting(Black_king, x, y);
            }
            else if (mal.Contains("queen"))
            {
                int r = int.Parse(mal.Substring(mal.Length - 1, 1));
                graphic_setting(Black_queen[r], x, y);
            }
        }
    }

    public void MovePredict(string mal, int x, int y)
    {
        Debug.Log("MovePredict : " + mal);
        if (mal.Contains("White"))
        {
            if (mal.Contains("pawn"))
            {
                try
                {
                    if (y == 1) //2칸 이동
                    {
                        if (chess_pan[x, y + 1] == "/")
                        {
                            graphic_setting(Instantiate(nemo), x, y + 1);
                            chess_nemo[x, y + 1] = "nemo";
                            if (chess_pan[x, y + 2] == "/")
                            {
                                graphic_setting(Instantiate(nemo), x, y + 2);
                                chess_nemo[x, y + 2] = "nemo";
                            }
                        }
                    }
                    else
                    {
                        if (chess_pan[x, y + 1] == "/")
                        {
                            graphic_setting(Instantiate(nemo), x, y + 1);
                            chess_nemo[x, y + 1] = "nemo";
                        }
                    }
                    if (x + 1 < 8 && y + 1 < 8)
                    {
                        if ((chess_pan[x + 1, y + 1]).Contains("Black")) //attack
                        {
                            GameObject nemo_red = Instantiate(nemo);
                            nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                            graphic_setting(nemo_red, x + 1, y + 1);
                            chess_nemo[x + 1, y + 1] = "nemo";
                        }
                    }
                    if (x - 1 > -1 && y + 1 < 8)
                    {

                        if ((chess_pan[x - 1, y + 1]).Contains("Black")) //attack
                        {
                            GameObject nemo_red = Instantiate(nemo);
                            nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                            graphic_setting(nemo_red, x - 1, y + 1);
                            chess_nemo[x - 1, y + 1] = "nemo";
                        }
                    }
                }
                catch (Exception e1) { }
            }
            else if (mal.Contains("knight"))
            {
                movepredict(x + 1, y + 2, "White");
                movepredict(x - 1, y + 2, "White");
                movepredict(x + 2, y + 1, "White");
                movepredict(x + 2, y - 1, "White");
                movepredict(x + 1, y - 2, "White");
                movepredict(x - 1, y - 2, "White");
                movepredict(x - 2, y + 1, "White");
                movepredict(x - 2, y - 1, "White");
            }
            else if (mal.Contains("bishop"))
            {
                movepredict2(x, y, 1, 1, "White");
                movepredict2(x, y, 1, -1, "White");
                movepredict2(x, y, -1, 1, "White");
                movepredict2(x, y, -1, -1, "White");
            }
            else if (mal.Contains("rook"))
            {
                movepredict2(x, y, 1, 0, "White");
                movepredict2(x, y, 0, 1, "White");
                movepredict2(x, y, -1, 0, "White");
                movepredict2(x, y, 0, -1, "White");
            }
            else if (mal.Contains("king"))
            {
                movepredict(x, y + 1, "White");
                movepredict(x, y - 1, "White");
                movepredict(x - 1, y - 1, "White");
                movepredict(x - 1, y, "White");
                movepredict(x - 1, y + 1, "White");
                movepredict(x + 1, y - 1, "White");
                movepredict(x + 1, y, "White");
                movepredict(x + 1, y + 1, "White");
            }
            else if (mal.Contains("queen"))
            {
                movepredict2(x, y, 1, 0, "White");
                movepredict2(x, y, 0, 1, "White");
                movepredict2(x, y, 1, 1, "White");
                movepredict2(x, y, -1, 0, "White");
                movepredict2(x, y, 0, -1, "White");
                movepredict2(x, y, -1, -1, "White");
                movepredict2(x, y, -1, 1, "White");
                movepredict2(x, y, 1, -1, "White");
            }
        }
        else
        {
            if (mal.Contains("pawn"))
            {
                try
                {
                    if (y == 6) //2칸 이동
                    {
                        if (chess_pan[x, y - 1] == "/")
                        {
                            graphic_setting(Instantiate(nemo), x, y - 1);
                            chess_nemo[x, y - 1] = "nemo";
                            if (chess_pan[x, y - 2] == "/")
                            {
                                graphic_setting(Instantiate(nemo), x, y - 2);
                                chess_nemo[x, y - 2] = "nemo";
                            }
                        }
                    }
                    else
                    {
                        if (chess_pan[x, y - 1] == "/")
                        {
                            graphic_setting(Instantiate(nemo), x, y - 1);
                            chess_nemo[x, y - 1] = "nemo";
                        }
                    }
                    if (x + 1 < 8 && y - 1 > -1)
                    {
                        if ((chess_pan[x + 1, y - 1]).Contains("White")) //attack
                        {
                            GameObject nemo_red = Instantiate(nemo);
                            nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                            graphic_setting(nemo_red, x + 1, y - 1);
                            chess_nemo[x + 1, y - 1] = "nemo";
                        }
                    }
                    if (x - 1 > -1 && y - 1 > -1)
                    {
                        if ((chess_pan[x - 1, y - 1]).Contains("White")) //attack
                        {
                            GameObject nemo_red = Instantiate(nemo);
                            nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                            graphic_setting(nemo_red, x - 1, y - 1);
                            chess_nemo[x - 1, y - 1] = "nemo";
                        }
                    }
                }
                catch (Exception e1) { }
            }
            else if (mal.Contains("knight"))
            {
                movepredict(x + 1, y + 2, "Black");
                movepredict(x - 1, y + 2, "Black");
                movepredict(x + 2, y + 1, "Black");
                movepredict(x + 2, y - 1, "Black");
                movepredict(x + 1, y - 2, "Black");
                movepredict(x - 1, y - 2, "Black");
                movepredict(x - 2, y + 1, "Black");
                movepredict(x - 2, y - 1, "Black");
            }
            else if (mal.Contains("bishop"))
            {
                movepredict2(x, y, 1, 1, "Black");
                movepredict2(x, y, 1, -1, "Black");
                movepredict2(x, y, -1, 1, "Black");
                movepredict2(x, y, -1, -1, "Black");
            }
            else if (mal.Contains("rook"))
            {
                movepredict2(x, y, 1, 0, "Black");
                movepredict2(x, y, 0, 1, "Black");
                movepredict2(x, y, -1, 0, "Black");
                movepredict2(x, y, 0, -1, "Black");
            }
            else if (mal.Contains("king"))
            {
                movepredict(x, y + 1, "Black");
                movepredict(x, y - 1, "Black");
                movepredict(x - 1, y - 1, "Black");
                movepredict(x - 1, y, "Black");
                movepredict(x - 1, y + 1, "Black");
                movepredict(x + 1, y - 1, "Black");
                movepredict(x + 1, y, "Black");
                movepredict(x + 1, y + 1, "Black");
            }
            else if (mal.Contains("queen"))
            {
                movepredict2(x, y, 1, 0, "Black");
                movepredict2(x, y, 0, 1, "Black");
                movepredict2(x, y, 1, 1, "Black");
                movepredict2(x, y, -1, 0, "Black");
                movepredict2(x, y, 0, -1, "Black");
                movepredict2(x, y, -1, -1, "Black");
                movepredict2(x, y, -1, 1, "Black");
                movepredict2(x, y, 1, -1, "Black");
            }
        }
    }
    public void movepredict(int x, int y, string team) //킹과 나이트의 움직임
    {
        try
        {
            if (chess_pan[x, y].Contains("/"))
            {
                graphic_setting(Instantiate(nemo), x, y);
                chess_nemo[x, y] = "nemo";
            }
            else if (chess_pan[x, y].Contains(team) == false)
            {
                GameObject nemo_red = Instantiate(nemo);
                nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                graphic_setting(nemo_red, x, y);
                chess_nemo[x, y] = "nemo";
            }
        }
        catch (Exception e1) { }
    }

    public void movepredict2(int x, int y, int x_plus, int y_plus, string team) //비숍, 룩, 퀸의 움직임
    {
        try
        {
            int search_x = x + x_plus;
            int search_y = y + y_plus;
            while (chess_pan[search_x, search_y].Contains("/"))
            {
                graphic_setting(Instantiate(nemo), search_x, search_y);
                chess_nemo[search_x, search_y] = "nemo";
                search_x += x_plus;
                search_y += y_plus;
            }
            if (chess_pan[search_x, search_y].Contains(team) == false)
            {
                GameObject nemo_red = Instantiate(nemo);
                nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                graphic_setting(nemo_red, search_x, search_y);
                chess_nemo[search_x, search_y] = "nemo";
            }
        }
        catch (Exception e1) { }
    }
    public void graphic_setting(GameObject obj, int x, int y) //이미지 표시
    {
        float change_x = x, change_y = y;
        change_x *= 104.75f;
        change_y *= 104.75f;
        change_x += -367.75f;
        change_y += -367.75f;

        RectTransform rectTran;
        rectTran = obj.GetComponent<RectTransform>();
        rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
        rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
        obj.transform.SetParent(chess_pan_imge.transform, false);
        obj.transform.localPosition = new Vector2(change_x, change_y);
    }

    public void pawn_upgrade(string pawn_mal, int x, int y)
    {
        int r = int.Parse(pawn_mal.Substring(pawn_mal.Length - 1, 1)) - 1;
        if (pawn_mal.Contains("White"))
        {
            if (player1.Contains("Black"))
            {
                Destroy(White_pawn[r]);
                White_queen[r + 1] = Instantiate(GameObject.Find("Canvas/black_king"));
                chess_pan[x, y] = "White_queen" + (r + 1).ToString();
                graphic_setting(White_queen[r + 1], x, y);
            }
            else
            {
                Destroy(White_pawn[r]);
                White_queen[r + 1] = Instantiate(GameObject.Find("Canvas/white_king"));
                chess_pan[x, y] = "White_queen" + (r + 1).ToString();
                graphic_setting(White_queen[r + 1], x, y);
            }
        }
        else
        {
            if (player1.Contains("Black"))
            {
                Destroy(Black_pawn[r]);
                Black_queen[r + 1] = Instantiate(GameObject.Find("Canvas/white_king"));
                chess_pan[x, y] = "Black_queen" + (r + 1).ToString();
                graphic_setting(Black_queen[r + 1], x, y);
            }
            else
            {
                Destroy(Black_pawn[r]);
                Black_queen[r + 1] = Instantiate(GameObject.Find("Canvas/black_king"));
                chess_pan[x, y] = "Black_queen" + (r + 1).ToString();
                graphic_setting(Black_queen[r + 1], x, y);
            }
        }
    }

    ArrayList check_pan_pre;

    // heuristic 함수들
    public int heuristic(int depth, string player, int alpha, int beta)
    {
        if (depth == 0)
        {
            return 0;
        }
        ArrayList mals = find_mal(player);
        if (player.Contains(AI)) //max
        {
            int temp = -99999999;
            bool check_cut = false;
            foreach (heuristic_imformation cur_mal in mals) //그말에 가능한 좌표
            {
                List<Vector2> mal_loca = heuristic_predict_location(cur_mal.mal, (int)cur_mal.loca.x, (int)cur_mal.loca.y, player); //그말에 가능한 좌표
                if (mal_loca.Count > 0)
                {
                    foreach (Vector2 loo_ca in mal_loca)
                    {
                        string temptemp = chess_pan[(int)loo_ca.x, (int)loo_ca.y].Trim();
                        string temptemp_pre = chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y];
                        if (temptemp.Contains("king")) //왕을 잡을시
                        {
                            int ccut = 0;
                            if (depth == 4)
                            {
                                ccut = 0;
                                pre_x = (int)cur_mal.loca.x;
                                pre_y = (int)cur_mal.loca.y;
                                ai.mal = cur_mal.mal;
                                ai.loca.x = loo_ca.x;
                                ai.loca.y = loo_ca.y;
                            }
                            else ccut = 1;
                            return 90000 - (ccut * 10000);
                        }
                        int rere_value = score(temptemp, depth); //attack 인지 확인
                        if (chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Contains("pawn") && loo_ca.y == 0)//폰승급
                        {
                            int r = int.Parse(cur_mal.mal.Substring(cur_mal.mal.Length - 1, 1)) - 1;
                            rere_value += 100;
                            chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "Black_queen" + (r + 1).ToString();
                            chess_pan[(int)loo_ca.x, (int)loo_ca.y] = chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Trim();
                            chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "/";
                        }
                        else
                        {
                            chess_pan[(int)loo_ca.x, (int)loo_ca.y] = chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Trim();
                            chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "/";
                        }
                        //if (pan_settings(chess_pan) == false) { rere_value += heuristic(depth - 1, PLAYER, alpha, beta); }
                        rere_value -= heuristic(depth - 1, PLAYER, alpha, beta);
                        if (rere_value > temp)
                        { //전값보다 이번 휴리스틱 값이 크면
                            temp = rere_value;
                            if (depth == 4)
                            {
                                pre_x = (int)cur_mal.loca.x;
                                pre_y = (int)cur_mal.loca.y;
                                ai.mal = cur_mal.mal;
                                ai.loca.x = loo_ca.x;
                                ai.loca.y = loo_ca.y;
                            }
                        }
                        chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = temptemp_pre;
                        chess_pan[(int)loo_ca.x, (int)loo_ca.y] = temptemp;
                        alpha = Mathf.Max(temp, alpha); //알파 값 찾기
                        if (beta <= alpha)
                        { //알파베타 가지치기
                            check_cut = true;
                            break;
                        }
                    }
                }
                if (check_cut)
                {
                    break;
                }
            }
            return temp;
        }
        else //min
        {
            int temp = 99999999;
            bool check_cut = false;
            foreach (heuristic_imformation cur_mal in mals) //자신의 모든 말 찾기
            {
                List<Vector2> mal_loca = heuristic_predict_location(cur_mal.mal, (int)cur_mal.loca.x, (int)cur_mal.loca.y, player); //그말에 가능한 좌표
                foreach (Vector2 loo_ca in mal_loca) //각 말에 대한 이동가능한 좌표
                {
                    string temptemp = chess_pan[(int)loo_ca.x, (int)loo_ca.y].Trim();
                    string temptemp_pre = chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y];
                    if (temptemp.Contains("king")) //왕을 잡을시
                    { return 90000; }
                    int rere_value = score(temptemp, depth); //attack 인지 확인
                    if (chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Contains("pawn") && loo_ca.y == 7) //폰승급
                    {
                        rere_value += 100;
                        int r = int.Parse(cur_mal.mal.Substring(cur_mal.mal.Length - 1, 1)) - 1;
                        chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "White_queen" + (r + 1).ToString();
                        chess_pan[(int)loo_ca.x, (int)loo_ca.y] = chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Trim();
                        chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "/";
                    }
                    else
                    {
                        chess_pan[(int)loo_ca.x, (int)loo_ca.y] = chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Trim();
                        chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "/";
                    }
                    //if (pan_settings(chess_pan) == false) {rere_value += heuristic(depth - 1, AI, alpha, beta);}
                    rere_value += heuristic(depth - 1, AI, alpha, beta);
                    rere_value *= -1;
                    temp = Mathf.Min(rere_value, temp);
                    chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = temptemp_pre;
                    chess_pan[(int)loo_ca.x, (int)loo_ca.y] = temptemp;
                    beta = Mathf.Min(temp, beta);
                    if (beta <= alpha)
                    {
                        check_cut = true;
                        break;
                    }
                }
                if (check_cut)
                {
                    break;
                }
            }
            return temp;
        }
    }

    public bool pan_settings(string[,] rr) // Transposition Table 사용
    {
        string temp = "";
        for (int i = 0; i < 8; i++)
        {
            for (int h = 0; h < 8; h++)
            {
                if (rr[h, i].Length > 2) { temp += rr[h, i].Substring(0, rr[h, i].Length - 1).Trim(); }
                else { temp += rr[h, i].Substring(0).Trim(); }
            }
        }
        foreach (string ppan in check_pan_pre)
        {
            if (temp.Contains(ppan))
            {
                return true;
            }
        }
        check_pan_pre.Add(temp);
        return false;
    }
    public int score(string mal, int depth) //점수 계산
    {
        int cut = 0;
        if (depth == 4) cut = 0;
        else if (depth == 2) cut = 1;
        if (mal.Contains("pawn"))
        {
            return 10 - (cut * 2);
        }
        else if (mal.Contains("knight"))
        {
            return 71 - (cut * 20);
        }
        else if (mal.Contains("bishop"))
        {
            return 31 - (cut * 7);
        }
        else if (mal.Contains("rook"))
        {
            return 101 - (cut * 40);
        }
        else if (mal.Contains("queen"))
        {
            return 351 - (cut * 40);
        }
        return 0;
    }

    public ArrayList find_mal(string mal_color)
    { //자신의 말 존재 확인
        ArrayList mals = new ArrayList();
        int cnt = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int h = 0; h < 8; h++)
            {
                if (chess_pan[h, i].Contains(mal_color))
                {
                    cnt++;
                    mals.Add(new heuristic_imformation(new Vector2(h, i), chess_pan[h, i]));
                }
                if (cnt == 16)
                {
                    return mals;
                }
            }
        }
        return mals;
    }

    public List<Vector2> heuristic_predict_location(string mal1, int x, int y, string turn)
    {
        List<Vector2> mal_locations = new List<Vector2>();
        if (mal1.Contains("pawn"))
        {
            mal_locations.AddRange(heuristic_movepredict_pawn(x, y, turn));
        }
        else if (mal1.Contains("knight"))
        {
            mal_locations.AddRange(heuristic_movepredict(x + 1, y + 2, turn));
            mal_locations.AddRange(heuristic_movepredict(x - 1, y + 2, turn));
            mal_locations.AddRange(heuristic_movepredict(x + 2, y + 1, turn));
            mal_locations.AddRange(heuristic_movepredict(x + 2, y - 1, turn));
            mal_locations.AddRange(heuristic_movepredict(x + 1, y - 2, turn));
            mal_locations.AddRange(heuristic_movepredict(x - 1, y - 2, turn));
            mal_locations.AddRange(heuristic_movepredict(x - 2, y + 1, turn));
            mal_locations.AddRange(heuristic_movepredict(x - 2, y - 1, turn));
        }
        else if (mal1.Contains("bishop"))
        {
            mal_locations.AddRange(heuristic_movepredict2(x, y, 1, 1, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, 1, -1, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, -1, 1, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, -1, -1, turn));
        }
        else if (mal1.Contains("rook"))
        {
            mal_locations.AddRange(heuristic_movepredict2(x, y, 1, 0, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, 0, 1, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, -1, 0, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, 0, -1, turn));
        }
        else if (mal1.Contains("king"))
        {
            mal_locations.AddRange(heuristic_movepredict(x, y + 1, turn));
            mal_locations.AddRange(heuristic_movepredict(x, y - 1, turn));
            mal_locations.AddRange(heuristic_movepredict(x - 1, y - 1, turn));
            mal_locations.AddRange(heuristic_movepredict(x - 1, y, turn));
            mal_locations.AddRange(heuristic_movepredict(x - 1, y + 1, turn));
            mal_locations.AddRange(heuristic_movepredict(x + 1, y - 1, turn));
            mal_locations.AddRange(heuristic_movepredict(x + 1, y, turn));
            mal_locations.AddRange(heuristic_movepredict(x + 1, y + 1, turn));
        }
        else if (mal1.Contains("queen"))
        {
            mal_locations.AddRange(heuristic_movepredict2(x, y, 1, 0, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, 0, 1, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, 1, 1, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, -1, 0, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, 0, -1, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, -1, -1, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, -1, 1, turn));
            mal_locations.AddRange(heuristic_movepredict2(x, y, 1, -1, turn));
        }

        return mal_locations;
    }

    public List<Vector2> heuristic_movepredict_pawn(int x, int y, string team) //폰의 움직임
    {
        List<Vector2> mal_locations1 = new List<Vector2>();
        if (team == "Black")
        {
            if (y - 2 >= 0)
            {
                if (y == 1 && chess_pan[x, y - 2] == "/") //2칸 전진
                {
                    mal_locations1.Add(new Vector2(x, y - 2));
                }
            }
            if (y - 1 >= 0)
            {
                if (chess_pan[x, y - 1] == "/")
                {
                    mal_locations1.Add(new Vector2(x, y - 1));
                }
                if (x - 1 >= 0)
                {
                    if (chess_pan[x - 1, y - 1] != "/" && chess_pan[x - 1, y - 1].Contains(team) == false)
                    {
                        mal_locations1.Add(new Vector2(x - 1, y - 1));
                    }
                }
                if (x + 1 < 8)
                {
                    if (chess_pan[x + 1, y - 1] != "/" && chess_pan[x + 1, y - 1].Contains(team) == false)
                    {
                        mal_locations1.Add(new Vector2(x + 1, y - 1));
                    }
                }
            }

        }
        else
        {
            if (y + 2 < 8)
            {
                if (y == 1 && chess_pan[x, y + 2] == "/") //2칸 전진
                {
                    mal_locations1.Add(new Vector2(x, y + 2));
                }
            }
            if (y + 1 < 8)
            {
                if (chess_pan[x, y + 1] == "/")
                {
                    mal_locations1.Add(new Vector2(x, y + 1));
                }
                if (x - 1 >= 0)
                {
                    if (chess_pan[x - 1, y + 1] != "/" && chess_pan[x - 1, y + 1].Contains(team) == false)
                    {
                        mal_locations1.Add(new Vector2(x - 1, y + 1));
                    }
                }
                if (x + 1 < 8)
                {
                    if (chess_pan[x + 1, y + 1] != "/" && chess_pan[x + 1, y + 1].Contains(team) == false)
                    {
                        mal_locations1.Add(new Vector2(x + 1, y + 1));
                    }
                }
            }
        }
        return mal_locations1;
    }

    public List<Vector2> heuristic_movepredict(int x, int y, string team) //킹과 나이트의 움직임
    {
        List<Vector2> mal_locations2 = new List<Vector2>();
        if ((x < 8 && x >= 0) && (y < 8 && y >= 0))
        {
            if (chess_pan[x, y].Contains("/"))
            {
                mal_locations2.Add(new Vector2(x, y));
            }
            else if (!chess_pan[x, y].Contains(team))
            {
                mal_locations2.Add(new Vector2(x, y));
            }
        }
        return mal_locations2;
    }

    public List<Vector2> heuristic_movepredict2(int x, int y, int x_plus, int y_plus, string team) //비숍, 룩, 퀸의 움직임
    {
        List<Vector2> mal_locations3 = new List<Vector2>();
        int search_x = x + x_plus;
        int search_y = y + y_plus;
        while ((search_x < 8 && search_x >= 0) && (search_y < 8 && search_y >= 0))
        {
            if ((chess_pan[search_x, search_y].Contains("/")) == false)
            {
                break;
            }
            mal_locations3.Add(new Vector2(search_x, search_y));
            search_x += x_plus;
            search_y += y_plus;
        }
        if ((search_x < 8 && search_x >= 0) && (search_y < 8 && search_y >= 0))
        {
            if ((chess_pan[search_x, search_y].Contains(team)) == false)
            {
                mal_locations3.Add(new Vector2(search_x, search_y));
            }
        }
        return mal_locations3;
    }
    public void exit()
    {
        SceneManager.LoadScene("Main");
    }

    public void check()
    {
        Square.SetActive(false);
        blackbackground.SetActive(false);
        AI = "Black";
        PLAYER = "White";
        if (White_tog.isOn)
        {
            player1 = "White";
            AIColor_Black.SetActive(true);
            PlayerColor_White.SetActive(true);
            for (int i = 0; i < 8; i++)
            {
                for (int h = 0; h < 8; h++)
                {
                    chess_pan[h, i] = "/";
                    chess_nemo[h, i] = "0";
                }
            }
            for (int i = 2; i < 10; i++)
            {
                White_queen[i - 1] = null;
                Black_queen[i - 1] = null;
            }
            initPosition();
            chess_pan_imge = GameObject.Find("Canvas/Image");
            for (int i = 0; i < 8; i++)
            {
                Black_pawn[i] = Instantiate(GameObject.Find("Canvas/black_pawn"));
                White_pawn[i] = Instantiate(GameObject.Find("Canvas/white_pawn"));
            }
            for (int i = 0; i < 2; i++)
            {
                Black_bishop[i] = Instantiate(GameObject.Find("Canvas/black_bishop"));
                Black_knight[i] = Instantiate(GameObject.Find("Canvas/black_knight2"));
                Black_rook[i] = Instantiate(GameObject.Find("Canvas/black_rook"));

                White_bishop[i] = Instantiate(GameObject.Find("Canvas/white_bishop"));
                White_knight[i] = Instantiate(GameObject.Find("Canvas/white_knight2"));
                White_rook[i] = Instantiate(GameObject.Find("Canvas/white_rook"));
            }
            Black_king = Instantiate(GameObject.Find("Canvas/black_queen"));
            Black_queen[0] = Instantiate(GameObject.Find("Canvas/black_king"));

            White_king = Instantiate(GameObject.Find("Canvas/white_queen"));
            White_queen[0] = Instantiate(GameObject.Find("Canvas/white_king"));
            init_graphic();
        }
        //temp_loca_chess_pre에 있는 말이 temp_loca_chess_next로 이동함
        else
        {
            player1 = "Black";
            AIColor_White.SetActive(true);
            PlayerColor_Black.SetActive(true);
            for (int i = 0; i < 8; i++)
            {
                for (int h = 0; h < 8; h++)
                {
                    chess_pan[h, i] = "/";
                    chess_nemo[h, i] = "0";
                }
            }
            for (int i = 2; i < 10; i++)
            {
                White_queen[i - 1] = null;
                Black_queen[i - 1] = null;
            }
            initPosition();
            chess_pan_imge = GameObject.Find("Canvas/Image");
            for (int i = 0; i < 8; i++)
            {
                Black_pawn[i] = Instantiate(GameObject.Find("Canvas/white_pawn"));
                White_pawn[i] = Instantiate(GameObject.Find("Canvas/black_pawn"));
            }
            for (int i = 0; i < 2; i++)
            {
                Black_bishop[i] = Instantiate(GameObject.Find("Canvas/white_bishop"));
                Black_knight[i] = Instantiate(GameObject.Find("Canvas/white_knight2"));
                Black_rook[i] = Instantiate(GameObject.Find("Canvas/white_rook"));

                White_bishop[i] = Instantiate(GameObject.Find("Canvas/black_bishop"));
                White_knight[i] = Instantiate(GameObject.Find("Canvas/black_knight2"));
                White_rook[i] = Instantiate(GameObject.Find("Canvas/black_rook"));
            }
            Black_king = Instantiate(GameObject.Find("Canvas/white_queen"));
            Black_queen[0] = Instantiate(GameObject.Find("Canvas/white_king"));

            White_king = Instantiate(GameObject.Find("Canvas/black_queen"));
            White_queen[0] = Instantiate(GameObject.Find("Canvas/black_king"));
            init_graphic();
            pre_x = 3;
            pre_y = 6;
            pre_loca.Add(new Vector2(3, 6));
            next_loca.Add(new Vector2(3, 4));
            Move(chess_pan[3, 6], 3, 4);
        }
    }
    // Update is called once per frame
    int cnttt = 0;
    private void Run()
    {
        while (true)
        {
            cnttt++;
            cnttt = cnttt % 4;
            Debug.Log(ai_text[cnttt]);
            System.Threading.Thread.Sleep(500);
        }
    }
    void Update()
    {
        ai_wait_text.text = "계산중...";
    }

    class heuristic_imformation
    {
        public Vector2 loca;
        public string mal;
        public heuristic_imformation()
        {
            loca = new Vector2();
            mal = "";
        }
        public heuristic_imformation(Vector2 lo, string ma)
        {
            loca = lo;
            mal = ma;
        }
    }

}