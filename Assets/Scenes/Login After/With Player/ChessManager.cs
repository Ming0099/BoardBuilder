using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChessManager : MonoBehaviour, IPointerClickHandler
{
    public Text RoomName;

    string[,] chess_pan = new string[8, 8];
    string[,] chess_nemo = new string[8, 8];
    GameObject chess_pan_imge;
    GameObject[] Black_pawn = new GameObject[8];
    GameObject[] Black_knight = new GameObject[10];
    GameObject[] Black_bishop = new GameObject[10];
    GameObject[] Black_rook = new GameObject[10];
    GameObject Black_king;
    GameObject[] Black_queen = new GameObject[9];

    GameObject[] White_pawn = new GameObject[8];
    GameObject[] White_knight = new GameObject[10];
    GameObject[] White_bishop = new GameObject[10];
    GameObject[] White_rook = new GameObject[10];
    GameObject White_king;
    GameObject[] White_queen = new GameObject[9];
    GameObject nemo;
    int pre_x, pre_y;

    public GameObject WaitPanel;
    public Text MatchingMessage;

    public Text Turn;

    public GameObject WinPanel;
    public Text WinText;

    public PhotonView PV;

    string player = "White";
    string myColor;

    ArrayList next_loca, pre_loca;
    GameObject login_information_;
    DateTime t;
    string date;

    public Text name1_text_;
    public Text name2_text_;
    void Start()
    {
        WinPanel.SetActive(false);
        next_loca = new ArrayList();
        pre_loca = new ArrayList();
        login_information_ = GameObject.Find("Login_");
        nemo = GameObject.FindGameObjectWithTag("temp");
        t = DateTime.Now;
        date = t.ToString("yyyy/MM/dd HH:mm:ss");
        RoomName.text = "<" + PhotonNetwork.CurrentRoom.Name.Substring(0, PhotonNetwork.CurrentRoom.Name.Length - 4) + ">";
        // 먼저 들어온 사람이 흰색
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            myColor = "White";
        }
        else
        {
            myColor = "Black";
        }
        init();
    }
    public void init()
    {
        nemo = GameObject.FindGameObjectWithTag("temp");
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
            Black_knight[i] = null;
            Black_bishop[i] = null;
            Black_rook[i] = null;
            White_knight[i] = null;
            White_bishop[i] = null;
            White_rook[i] = null;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (myColor == player)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(),
                eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
                return;
            localCursor.x = localCursor.x + 427;
            localCursor.y = localCursor.y + 427;
            double x = (localCursor.x - 8.67616) / 104.80004;
            double y = (localCursor.y - 9.060577) / 104.80004;
            Debug.Log("네모칸 안에는 " + chess_pan[(int)x, (int)y]);
            PV.RPC("TestRPC", RpcTarget.All, x, y);
            
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
                WinPanel.SetActive(true);
                login_information_.GetComponent<Login_information>().set_upgrade_chess(pre_loca, next_loca, "White", date);
                if (myColor == "White")
                {
                    WinText.text = "승리";
                }
                else
                {
                    WinText.text = "패배";
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
                WinPanel.SetActive(true);
                login_information_.GetComponent<Login_information>().set_upgrade_chess(pre_loca, next_loca, "White", date);
                if (myColor == "Black")
                {
                    WinText.text = "승리";
                }
                else
                {
                    WinText.text = "패배";
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
                if(y == 7)
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
                if(y == 0)
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
                    if(x == 7)
                    {
                        if ((chess_pan[x - 1, y + 1]).Contains("Black")) //attack
                        {
                            GameObject nemo_red = Instantiate(nemo);
                            nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                            graphic_setting(nemo_red, x - 1, y + 1);
                            chess_nemo[x - 1, y + 1] = "nemo";
                        }
                    }
                    else
                    {
                        if ((chess_pan[x + 1, y + 1]).Contains("Black")) //attack
                        {
                            GameObject nemo_red = Instantiate(nemo);
                            nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                            graphic_setting(nemo_red, x + 1, y + 1);
                            chess_nemo[x + 1, y + 1] = "nemo";
                        }
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
                    if(x == 7)
                    {
                        if ((chess_pan[x - 1, y - 1]).Contains("White")) //attack
                        {
                            GameObject nemo_red = Instantiate(nemo);
                            nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                            graphic_setting(nemo_red, x - 1, y - 1);
                            chess_nemo[x - 1, y - 1] = "nemo";
                        }
                    }
                    else
                    {
                        if ((chess_pan[x + 1, y - 1]).Contains("White")) //attack
                        {
                            GameObject nemo_red = Instantiate(nemo);
                            nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                            graphic_setting(nemo_red, x + 1, y - 1);
                            chess_nemo[x + 1, y - 1] = "nemo";
                        }
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
        catch (Exception e1)
        {

        }
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
        catch (Exception e1)
        {

        }

    }
    public void graphic_setting(GameObject obj, int x, int y)
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
    public void pawn_upgrade(string pawn_mal, int x, int y)
    {
        int r = int.Parse(pawn_mal.Substring(pawn_mal.Length - 1, 1)) - 1;
        if (pawn_mal.Contains("White"))
        {
            if (player.Contains("Black"))
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
            if (player.Contains("Black"))
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


    private bool startFlag = false;
    private float timer = 0f;
    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && startFlag == false)
        {
            WaitPanel.SetActive(false);
            startFlag = true;
            Turn.text = PhotonNetwork.PlayerList[0].NickName + "의 턴";
            name1_text_.text = PhotonNetwork.PlayerList[0].NickName;
            name2_text_.text = PhotonNetwork.PlayerList[1].NickName;
        }
        else if (startFlag == false) // 매칭 대기 메시지
        {
            timer += Time.deltaTime;
            if (timer > 0.6f)
            {
                if (MatchingMessage.text.Length > 5)
                {
                    MatchingMessage.text = "매칭중";
                }
                else
                {
                    MatchingMessage.text = MatchingMessage.text + ".";
                }
                timer = 0f;
            }
        }
    }

    public void WaitingBackEvent()
    {
        PhotonNetwork.LeaveRoom();
        Destroy(NetworkManager.myObj);
        SceneManager.LoadScene("Lobby");
    }

    [PunRPC]
    void TestRPC(double x, double y)
    {
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

            Move(chess_pan[pre_x, pre_y], (int)x, (int)y);
            if (player.Contains("White"))
            {
                player = "Black";
                Turn.text = PhotonNetwork.PlayerList[1].NickName+"의 턴";

            }
            else
            {
                player = "White";
                Turn.text = PhotonNetwork.PlayerList[0].NickName+"의 턴";

            }
        }
        else if ((chess_pan[(int)x, (int)y].Contains("/") == false) && (chess_pan[(int)x, (int)y].Contains(player))) //다른 말 클릭시
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
            pre_x = (int)x;
            pre_y = (int)y;
            Debug.Log("2 / 네모칸 안에는 " + chess_pan[(int)x, (int)y]);
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
                {
                    if (chess_nemo[h, i].Contains("nemo"))
                    {
                        chess_nemo[h, i] = "0";
                    }
                }
            }
        }
    }

    public void ToHome()
    {
        // 네트워크 객체 파괴 (연결종료)
        Destroy(NetworkManager.myObj);
        SceneManager.LoadScene("Main");
    }
}
