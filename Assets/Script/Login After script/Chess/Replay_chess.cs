using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Database;
using Firebase.Unity;
using Firebase;
using Firebase.Auth;

public class Replay_chess : MonoBehaviour, IPointerClickHandler
{
    GameObject login_information_;
    GameObject enter;

    ArrayList a1a1;
    public Toggle to;
    public InputField count_input;
    public GameObject blackimg;
    public Text end_count;
    int ccnt, end_ccnt, pre_x, pre_y;
    Dead_mal_info alive = new Dead_mal_info();
    string cur_play = "White";

    ArrayList pre_loca, next_loca, predict_next_loca, predict_pre_loca;

    string[,] chess_pan = new string[8, 8];
    string[,] chess_nemo = new string[8, 8];
    string user_team;
    string click_team, heu_team, heu_other;

    GameObject nemo;

    ArrayList heu_and_dead;

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

    bool custom_check = false;
    int page;
    bool heu_check = false;
    void Start()
    {
        heu_and_dead = new ArrayList();
        predict_next_loca = new ArrayList();
        predict_pre_loca = new ArrayList();
        nemo = GameObject.FindGameObjectWithTag("temp");
        blackimg.SetActive(false);
        login_information_ = GameObject.Find("Login_");
        enter = GameObject.Find("entering");
        string anwser = enter.GetComponent<chess_enter>().d_date;
        ArrayList temptemp = login_information_.GetComponent<Login_information>().return_chessdate();
        GameObject.Find("sel_col_canvas/right").transform.localScale = new Vector2(-1, 1);
        GameObject.Find("Canvas/pre_btn").transform.localScale = new Vector2(-1, 1);
        for (int i = 0; i < temptemp.Count; i++)
        {
            if (anwser.Equals(temptemp[i].ToString()))
            {
                page = i;
                break;
            }
        }
        Debug.Log(page + " 번쨰 파일");
        ccnt = 1;
        ArrayList aa = login_information_.GetComponent<Login_information>().return_chesslist();
        a1a1 = (ArrayList)aa[page]; //다음 원하는 리플레이 정보가져오기
        pre_loca = (ArrayList)a1a1[0];
        next_loca = (ArrayList)a1a1[1];
        user_team = login_information_.GetComponent<Login_information>().chess_user_team[page];
        init_team_mal();
        Debug.Log("팀 : " + user_team);
        end_ccnt = ((ArrayList)a1a1[0]).Count;
        count_input.text = ccnt.ToString();
        end_count.text = "/ " + end_ccnt.ToString();
        dol_graphic();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (custom_check)
        {
            if (user_team.Contains("Black"))
            {
                if (cur_play.Contains("Black")) { click_team = "White"; }
                else { click_team = "Black"; }
            }
            else
            {
                if (cur_play.Contains("Black")) { click_team = "Black"; }
                else { click_team = "White"; }
            }
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(),
            eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
                return;
            localCursor.x = localCursor.x + 427;
            localCursor.y = localCursor.y + 427;
            double x = (localCursor.x - 8.67616) / 104.80004;
            double y = (localCursor.y - 9.060577) / 104.80004;
            Debug.Log("클릭한 곳 : " + (int)x + " , " + (int)y);
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
                predict_pre_loca.Add(new Vector2(pre_x, pre_y));
                predict_next_loca.Add(new Vector2((int)x, (int)y));
                Move(chess_pan[(int)pre_x, (int)pre_y], (int)x, (int)y);
                if (cur_play.Contains("Black")) { cur_play = "White"; }
                else { cur_play = "Black"; }
            }
            else if ((chess_pan[(int)x, (int)y].Contains("/") == false) && (chess_pan[(int)x, (int)y].Contains(click_team))) //다른 말 클릭시
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
    }
    public void right_click()
    {
        predict_next_loca = new ArrayList();
        predict_pre_loca = new ArrayList();
        ccnt++;
        if (ccnt > end_ccnt) ccnt = end_ccnt;
        Debug.Log(ccnt.ToString());
        count_input.text = (ccnt).ToString();
        cur_play = "White";
        dol_graphic();
    }
    public void left_click()
    {
        predict_next_loca = new ArrayList();
        predict_pre_loca = new ArrayList();
        ccnt--;
        if (ccnt < 1) ccnt = 1;
        Debug.Log(ccnt.ToString());
        count_input.text = (ccnt).ToString();
        cur_play = "White";
        dol_graphic();
    }
    public void counter_input()
    {
        predict_next_loca = new ArrayList();
        predict_pre_loca = new ArrayList();
        ccnt = int.Parse(count_input.text);
        if(ccnt <= 0) { ccnt = 1; count_input.text = (1).ToString(); }
        cur_play = "White";
        dol_graphic();
    }
    public void dol_graphic()
    {
        GameObject[] temp_mal = GameObject.FindGameObjectsWithTag("nemo1"); //판위의 네모모양이 있는 개수
        for (int i = 0; i < temp_mal.Length; i++)
        {
            Destroy(temp_mal[i]); //네모 칸 다 파괴
        }
        init_team_mal();
        for (int i=0;i<ccnt; i++)
        {
            Vector2 pp = (Vector2)pre_loca[i];
            pre_x = (int)pp.x;
            pre_y = (int)pp.y;
            Vector2 nn = (Vector2)next_loca[i];
            Move(chess_pan[pre_x, pre_y], (int)nn.x, (int)nn.y);
            if (cur_play.Contains("Black")) { cur_play = "White"; }
            else { cur_play = "Black"; }
        }
    }
    public void next_btn()
    {
        if (user_team.Contains("Black"))
        {
            if (cur_play.Contains("Black")) { heu_team = "White";  heu_other = "Black"; cur_play = "White"; }
            else { heu_team = "Black"; heu_other = "White"; cur_play = "Black"; }
        }
        else
        {
            if (cur_play.Contains("Black")) { heu_team = "Black"; heu_other = "White"; cur_play = "White"; }
            else { heu_team = "White"; heu_other = "Black"; cur_play = "Black"; }
        }
        int total = heuristic(heuristic_cnt, heu_team, -99999999, 99999999);
        predict_pre_loca.Add(new Vector2(pre_x, pre_y));
        predict_next_loca.Add(ai.loca);
        heu_check = true;
        Debug.Log(ai.mal + " / " +(int)ai.loca.x +" , "+ (int)ai.loca.y);
        Move(ai.mal, (int)ai.loca.x, (int)ai.loca.y);
    }
    public void pre_btn()
    { 
        if(predict_next_loca.Count > 0)
        {
            if (user_team.Contains("Black"))
            {
                if (cur_play.Contains("Black")) { heu_team = "White"; heu_other = "Black"; cur_play = "White"; }
                else { heu_team = "Black"; heu_other = "White"; cur_play = "Black"; }
            }
            else
            {
                if (cur_play.Contains("Black")) { heu_team = "Black"; heu_other = "White"; cur_play = "White"; }
                else { heu_team = "White"; heu_other = "Black"; cur_play = "Black"; }
            }
            alive = new Dead_mal_info();
            Vector2 next1 = (Vector2)predict_next_loca[predict_next_loca.Count - 1];
            Vector2 pre1 = (Vector2)predict_pre_loca[predict_pre_loca.Count - 1];
            bool alive_gogo = false;
            foreach(Dead_mal_info temmp in heu_and_dead)
            {
                if (predict_next_loca.Count == temmp.num)
                {
                    alive.loca.x = temmp.loca.x;
                    alive.loca.y = temmp.loca.y;
                    alive.mal = temmp.mal;
                    alive.num = temmp.num;
                    alive_gogo = true;
                    break;
                }
            }
            pre_x = (int)next1.x;
            pre_y = (int)next1.y;
            predict_next_loca.RemoveAt(predict_next_loca.Count - 1);
            predict_pre_loca.RemoveAt(predict_pre_loca.Count - 1);
            Move(chess_pan[(int)next1.x, (int)next1.y], (int)pre1.x, (int)pre1.y);
            if(alive_gogo) //살려야한다
            {
                heu_and_dead.RemoveAt(heu_and_dead.Count - 1);
                let_out();
            }
        }
    }
    public void let_out()
    {
        Debug.Log("죽은 말 : " + alive.mal + " 살릴 위치 : " + (int)alive.loca.x +" , "+ (int)alive.loca.y + " 현재 팀 : " + user_team);
        chess_pan[(int)alive.loca.x, (int)alive.loca.y] = alive.mal;
        int alive_num = int.Parse(alive.mal.Substring(alive.mal.Length - 1, 1));
        Debug.Log("숫자 : " + alive_num);
        if (user_team.Contains("Black"))
        {
            if (alive.mal.Contains("Black"))
            {
                if (alive.mal.Contains("pawn"))
                {
                    alive_num -= 1;
                    White_pawn[alive_num] = Instantiate(GameObject.Find("Canvas/white_pawn"));
                    graphic_setting(White_pawn[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("rook"))
                {
                    White_rook[alive_num] = Instantiate(GameObject.Find("Canvas/white_rook"));
                    graphic_setting(White_rook[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("king"))
                {
                    White_king = Instantiate(GameObject.Find("Canvas/white_king"));
                    graphic_setting(White_king, (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("knight"))
                {
                    White_knight[alive_num] = Instantiate(GameObject.Find("Canvas/white_knight2"));
                    graphic_setting(White_knight[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("bishop"))
                {
                    White_bishop[alive_num] = Instantiate(GameObject.Find("Canvas/white_bishop"));
                    graphic_setting(White_bishop[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("queen"))
                {
                    White_queen[alive_num] = Instantiate(GameObject.Find("Canvas/white_queen"));
                    graphic_setting(White_queen[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
            }
            else
            {
                if (alive.mal.Contains("pawn"))
                {
                    alive_num -= 1;
                    Black_pawn[alive_num] = Instantiate(GameObject.Find("Canvas/black_pawn"));
                    graphic_setting(Black_pawn[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("rook"))
                {
                    Black_rook[alive_num] = Instantiate(GameObject.Find("Canvas/black_rook"));
                    graphic_setting(Black_rook[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("king"))
                {
                    Black_king = Instantiate(GameObject.Find("Canvas/black_king"));
                    graphic_setting(Black_king, (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("knight"))
                {
                    Black_knight[alive_num] = Instantiate(GameObject.Find("Canvas/black_knight2"));
                    graphic_setting(Black_knight[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("bishop"))
                {
                    Black_bishop[alive_num] = Instantiate(GameObject.Find("Canvas/black_bishop"));
                    graphic_setting(Black_bishop[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("queen"))
                {
                    Black_queen[alive_num] = Instantiate(GameObject.Find("Canvas/black_queen"));
                    graphic_setting(Black_queen[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
            }
        }
        else
        {
            if (alive.mal.Contains("Black"))
            {
                if (alive.mal.Contains("pawn"))
                {
                    alive_num -= 1;
                    Black_pawn[alive_num] = new GameObject();
                    //Black_pawn[alive_num] = Instantiate(GameObject.Find("Canvas/black_pawn"));
                    Black_pawn[alive_num] = Instantiate((Resources.Load("preFab/black_pawn", typeof(GameObject)) as GameObject));
                    graphic_setting(Black_pawn[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("rook"))
                {
                    Black_rook[alive_num] = new GameObject();
                    //Black_rook[alive_num] = Instantiate(GameObject.Find("Canvas/black_rook"));
                    Black_rook[alive_num] = Instantiate((Resources.Load("preFab/black_rook", typeof(GameObject)) as GameObject));
                    graphic_setting(Black_rook[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("king"))
                {
                    Black_king = new GameObject();
                    //Black_king = Instantiate(GameObject.Find("Canvas/black_king"));
                    Black_king = Instantiate((Resources.Load("preFab/black_king", typeof(GameObject)) as GameObject));
                    graphic_setting(Black_king, (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("knight"))
                {
                    Black_knight[alive_num] = new GameObject();
                    //Black_knight[alive_num] = Instantiate(GameObject.Find("Canvas/black_knight2"));
                    Black_knight[alive_num] = Instantiate((Resources.Load("preFab/black_knight2", typeof(GameObject)) as GameObject));
                    graphic_setting(Black_knight[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("bishop"))
                {
                    Black_bishop[alive_num] = new GameObject();
                    //Black_bishop[alive_num] = Instantiate(GameObject.Find("Canvas/black_bishop"));
                    Black_bishop[alive_num] = Instantiate((Resources.Load("preFab/black_bishop", typeof(GameObject)) as GameObject));
                    graphic_setting(Black_bishop[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("queen"))
                {
                    Black_queen[alive_num] = new GameObject();
                    //Black_queen[alive_num] = Instantiate(GameObject.Find("Canvas/black_queen"));
                    Black_queen[alive_num] = Instantiate((Resources.Load("preFab/black_queen", typeof(GameObject)) as GameObject));
                    graphic_setting(Black_queen[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
            }
            else
            {
                if (alive.mal.Contains("pawn"))
                {
                    alive_num -= 1;
                    White_pawn[alive_num] = new GameObject();
                    //White_pawn[alive_num] = Instantiate(GameObject.Find("Canvas/white_pawn"));
                    White_pawn[alive_num] = Instantiate((Resources.Load("preFab/white_pawn", typeof(GameObject)) as GameObject));
                    graphic_setting(White_pawn[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("rook"))
                {
                    White_rook[alive_num] = new GameObject();
                    //White_rook[alive_num] = Instantiate(GameObject.Find("Canvas/white_rook"));
                    White_rook[alive_num] = Instantiate((Resources.Load("preFab/white_rook", typeof(GameObject)) as GameObject));
                    graphic_setting(White_rook[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("king"))
                {
                    White_king = new GameObject();
                    //White_king = Instantiate(GameObject.Find("Canvas/white_king"));
                    White_king = Instantiate((Resources.Load("preFab/white_king", typeof(GameObject)) as GameObject));
                    graphic_setting(White_king, (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("knight"))
                {
                    White_knight[alive_num] = new GameObject();
                    //White_knight[alive_num] = Instantiate(GameObject.Find("Canvas/white_knight2"));
                    White_knight[alive_num] = Instantiate((Resources.Load("preFab/white_knight2", typeof(GameObject)) as GameObject));
                    graphic_setting(White_knight[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("bishop"))
                {
                    White_bishop[alive_num] = new GameObject();
                    //White_bishop[alive_num] = Instantiate(GameObject.Find("Canvas/white_bishop"));
                    White_bishop[alive_num] = Instantiate((Resources.Load("preFab/white_bishop", typeof(GameObject)) as GameObject));
                    graphic_setting(White_bishop[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
                else if (alive.mal.Contains("queen"))
                {
                    White_queen[alive_num] = new GameObject();
                    //White_queen[alive_num] = Instantiate(GameObject.Find("/white_queen"));
                    White_queen[alive_num] = Instantiate((Resources.Load("preFab/white_queen", typeof(GameObject)) as GameObject));
                    graphic_setting(White_queen[alive_num], (int)alive.loca.x, (int)alive.loca.y);
                }
            }
        }
    }
    public void Move(string mal, int x, int y)
    {
        Debug.Log("말 : " + mal);
        if (chess_pan[x, y].Contains("/") == false) //공격
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
    public void attack(int x, int y, string cur_player) //공격당한 말 삭제
    {
        if (cur_player.Contains("White"))
        {
            string other_mal = chess_pan[x, y];
            Dead_mal_info tempp = new Dead_mal_info();
            tempp.mal = other_mal;
            tempp.loca = new Vector2(x, y);
            tempp.num = predict_next_loca.Count;
            heu_and_dead.Add(tempp);
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
                if (user_team.Contains("Black"))
                {
                    Debug.Log("흑의 승리");
                }
                else
                {
                    Debug.Log("백의 승리");
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
                if (user_team.Contains("Black"))
                {
                    Debug.Log("백의 승리");
                }
                else
                {
                    Debug.Log("흑의 승리");
                }

            }
            else if (other_mal.Contains("queen"))
            {
                int r = int.Parse(other_mal.Substring(other_mal.Length - 1, 1));
                Destroy(White_queen[r]);
            }
        }
    }
    public void pawn_upgrade(string pawn_mal, int x, int y)
    {
        int r = int.Parse(pawn_mal.Substring(pawn_mal.Length - 1, 1)) - 1;
        if (pawn_mal.Contains("White"))
        {
            if (user_team.Contains("Black"))
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
            if (user_team.Contains("Black"))
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

    heuristic_imformation ai = new heuristic_imformation();
    int heuristic_cnt = 4;
    ArrayList same_pan_check =new ArrayList();
    public int heuristic(int depth, string player, int alpha, int beta)
    {
        if (depth == 0)
        {
            return 0;
        }
        ArrayList mals = find_mal(player);
        if (player.Contains(heu_team)) //max
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
                            chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "Black_queen" + (r + 1).ToString();
                            rere_value += 100;
                            chess_pan[(int)loo_ca.x, (int)loo_ca.y] = chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Trim();
                            chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "/";
                        }
                        else
                        {
                            chess_pan[(int)loo_ca.x, (int)loo_ca.y] = chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Trim();
                            chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "/";
                        }
                        //if (pan_settings(chess_pan) == false) { rere_value += heuristic(depth - 1, PLAYER, alpha, beta); }
                        rere_value += heuristic(depth - 1, heu_other, alpha, beta);
                        bool ssame_check = false;
                        if (rere_value > temp)
                        { //전값보다 이번 휴리스틱 값이 크면
                            temp = rere_value;
                            if(depth == heuristic_cnt && heuristic_same_check())
                            {
                                System.Random rand = new System.Random();
                                heuristic_cnt = rand.Next(2, 6);
                                temp = -99999999;
                                ssame_check = true;
                            }
                            if (depth == heuristic_cnt && !ssame_check)
                            {
                                pre_x = (int)cur_mal.loca.x;
                                pre_y = (int)cur_mal.loca.y;
                                ai.mal = cur_mal.mal;
                                ai.loca.x = loo_ca.x;
                                ai.loca.y = loo_ca.y;
                                ssame_check = true;
                                //중복체크 
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
                    { return -900; }
                    int rere_value = score(temptemp, depth); //attack 인지 확인
                    if (chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Contains("pawn") && loo_ca.y == 7) //폰승급
                    {
                        int r = int.Parse(cur_mal.mal.Substring(cur_mal.mal.Length - 1, 1)) - 1;
                        chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "White_queen" + (r + 1).ToString();
                        rere_value += 100;
                        chess_pan[(int)loo_ca.x, (int)loo_ca.y] = chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Trim();
                        chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "/";
                    }
                    else
                    {
                        chess_pan[(int)loo_ca.x, (int)loo_ca.y] = chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y].Trim();
                        chess_pan[(int)cur_mal.loca.x, (int)cur_mal.loca.y] = "/";
                    }
                    //if (pan_settings(chess_pan) == false) {rere_value += heuristic(depth - 1, AI, alpha, beta);}
                    rere_value += heuristic(depth - 1, heu_team, alpha, beta);
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
    public bool heuristic_same_check()
    {
        string str = "";
        for(int i=0;i<8; i++)
        {
            for (int h = 0; h < 8; h++)
            {
                str += chess_pan[h, i];
            }
        }
        str = str.Trim();
        foreach(string search in same_pan_check)
        {
            string ppp = search.Trim();
            if (ppp.Contains(str)) { return true; }
        }
        same_pan_check.Add(str);
        return false;
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

    public void init_team_mal()
    {
        if (user_team.Contains("White"))
        {
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
        else
        {
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
                            graphic_setting(Instantiate(nemo), x, y + 1, 1);
                            chess_nemo[x, y + 1] = "nemo";
                            if (chess_pan[x, y + 2] == "/")
                            {
                                graphic_setting(Instantiate(nemo), x, y + 2, 1);
                                chess_nemo[x, y + 2] = "nemo";
                            }
                        }
                    }
                    else
                    {
                        if (chess_pan[x, y + 1] == "/")
                        {
                            graphic_setting(Instantiate(nemo), x, y + 1, 1);
                            chess_nemo[x, y + 1] = "nemo";
                        }
                    }
                    if ((chess_pan[x + 1, y + 1]).Contains("Black")) //attack
                    {
                        GameObject nemo_red = Instantiate(nemo);
                        nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                        graphic_setting(nemo_red, x + 1, y + 1, 1);
                        chess_nemo[x + 1, y + 1] = "nemo";
                    }
                    if ((chess_pan[x - 1, y + 1]).Contains("Black")) //attack
                    {
                        GameObject nemo_red = Instantiate(nemo);
                        nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                        graphic_setting(nemo_red, x - 1, y + 1, 1);
                        chess_nemo[x - 1, y + 1] = "nemo";
                    }
                }
                catch (Exception e) { }
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
                            graphic_setting(Instantiate(nemo), x, y - 1, 1);
                            chess_nemo[x, y - 1] = "nemo";
                            if (chess_pan[x, y - 2] == "/")
                            {
                                graphic_setting(Instantiate(nemo), x, y - 2, 1);
                                chess_nemo[x, y - 2] = "nemo";
                            }
                        }
                    }
                    else
                    {
                        if (chess_pan[x, y - 1] == "/")
                        {
                            graphic_setting(Instantiate(nemo), x, y - 1, 1);
                            chess_nemo[x, y - 1] = "nemo";
                        }
                    }
                    if ((chess_pan[x + 1, y - 1]).Contains("White")) //attack
                    {
                        GameObject nemo_red = Instantiate(nemo);
                        nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                        graphic_setting(nemo_red, x + 1, y - 1, 1);
                        chess_nemo[x + 1, y - 1] = "nemo";
                    }
                    if ((chess_pan[x - 1, y - 1]).Contains("White")) //attack
                    {
                        GameObject nemo_red = Instantiate(nemo);
                        nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                        graphic_setting(nemo_red, x - 1, y - 1, 1);
                        chess_nemo[x - 1, y - 1] = "nemo";
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
                graphic_setting(Instantiate(nemo), x, y, 1);
                chess_nemo[x, y] = "nemo";
            }
            else if (chess_pan[x, y].Contains(team) == false)
            {
                GameObject nemo_red = Instantiate(nemo);
                nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                graphic_setting(nemo_red, x, y, 1);
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
                graphic_setting(Instantiate(nemo), search_x, search_y, 1);
                chess_nemo[search_x, search_y] = "nemo";
                search_x += x_plus;
                search_y += y_plus;
            }
            if (chess_pan[search_x, search_y].Contains(team) == false)
            {
                GameObject nemo_red = Instantiate(nemo);
                nemo_red.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); //네모 모양을 빨간색으로 바꿔라
                graphic_setting(nemo_red, search_x, search_y, 1);
                chess_nemo[search_x, search_y] = "nemo";
            }
        }
        catch (Exception e1) { }
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
            graphic_setting(Black_pawn[i], i, 6, 0);
            graphic_setting(White_pawn[i], i, 1, 0);
        }
        graphic_setting(Black_knight[0], 1, 7, 0);
        graphic_setting(Black_bishop[0], 2, 7, 0);
        graphic_setting(Black_rook[0], 0, 7, 0);
        graphic_setting(Black_knight[1], 6, 7, 0);
        graphic_setting(Black_bishop[1], 5, 7, 0);
        graphic_setting(Black_rook[1], 7, 7, 0);

        graphic_setting(White_knight[0], 1, 0, 0);
        graphic_setting(White_bishop[0], 2, 0, 0);
        graphic_setting(White_rook[0], 0, 0, 0);
        graphic_setting(White_knight[1], 6, 0, 0);
        graphic_setting(White_bishop[1], 5, 0, 0);
        graphic_setting(White_rook[1], 7, 0, 0);

        graphic_setting(Black_king, 4, 7, 0);
        graphic_setting(Black_queen[0], 3, 7, 0);

        graphic_setting(White_king, 4, 0, 0);
        graphic_setting(White_queen[0], 3, 0, 0);
    }
    public void graphic_setting(GameObject obj, int x, int y, int sel = 0) //이미지 표시
    {
        float change_x = x, change_y = y;
        change_x *= 104.75f;
        change_y *= 104.75f;
        change_x += -367.75f;
        change_y += -367.75f;

        RectTransform rectTran;
        obj.GetComponent<RectTransform>(); 
        rectTran = obj.GetComponent<RectTransform>();
        rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
        rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
        obj.transform.SetParent(chess_pan_imge.transform, false);
        obj.transform.localPosition = new Vector2(change_x, change_y);
        if(sel == 1) //nemo
        {
            obj.tag = "temp";
        }
        else{ obj.tag = "nemo1"; } //mal
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
    class Dead_mal_info
    {
        public Vector2 loca;
        public string mal;
        public int num;
        public Dead_mal_info()
        {
            num = -1;
            loca = new Vector2();
            mal = "";
        }
        public Dead_mal_info(Vector2 lo, string ma, int counter) //죽은 말 찾기
        {
            num = counter;
            loca = lo;
            mal = ma;
        }
    }
    public void toggle_check()
    {
        if (to.isOn)
        {
            blackimg.SetActive(true);
            custom_check = true;
        }
        else
        {
            blackimg.SetActive(false);
            custom_check = false;
        }
    }
    public void exit_back()
    {
        Destroy(GameObject.Find("entering"));
        SceneManager.LoadScene("SelectChess");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
