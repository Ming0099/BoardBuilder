using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OmokManager : MonoBehaviourPunCallbacks, IPointerDownHandler
{
    public Text RoomName;
    public GameObject WaitPanel;
    public Text MatchingMessage;

    public GameObject black_dol;
    public GameObject white_dol;
    private GameObject myStone;

    private string nowColor;
    private string myColor;
    private bool myTurn;

    public Text Turn;

    public GameObject WinPanel;
    public Text WinText;

    public ArrayList Omok_play_data;

    DateTime t;
    string date;
    string total1;
    GameObject login_information_;
    public Text name1_text_;
    public Text name2_text_;

    private void Start()
    {
        t = DateTime.Now;
        date = t.ToString("yyyy/MM/dd HH:mm:ss");
        login_information_ = GameObject.Find("Login_");
        string nn = login_information_.GetComponent<Login_information>().return_name();

        Omok_play_data = new ArrayList();

        WinPanel.SetActive(false);
        RoomName.text = "<" + PhotonNetwork.CurrentRoom.Name.Substring(0, PhotonNetwork.CurrentRoom.Name.Length-4) + ">";
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        //먼저 들어온 사람이 흑색
        {
            myColor = "black";
            myTurn = true;
            myStone = black_dol;

        }
        else
        {
            myColor = "white";
            myTurn = false;
            myStone = white_dol;

        }
        nowColor = "black";
    }

    public PhotonView PV;
    public Text PositionText;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down");
    }

    [PunRPC]
    void ChatRPC(string msg)
    {
        PositionText.text = msg;
    }

    [PunRPC]
    void OmokPlay(string stone, int x, int y, double localx, double localy)
    {
        if (array[x, y] == 0)
        {
            RectTransform rectTran;
            rectTran = myStone.GetComponent<RectTransform>();
            rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
            rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
            double return_x = localx - 427;
            double return_y = localy - 427;
            var temp = Instantiate(myStone);
            if (nowColor == "black")
            {
                temp = Instantiate(black_dol);
            }
            else if(nowColor == "white")
            {
                temp = Instantiate(white_dol);
            }
            temp.transform.localScale = Vector2.one;
            temp.transform.SetParent(GameObject.Find("Canvas/OmokPan").transform, false);
            temp.transform.localPosition = new Vector2((float)return_x + 20f, (float)return_y + 20f);
            if(nowColor == "black")
            {
                array[x, y] = 1;
            }
            else if(nowColor == "white")
            {
                array[x, y] = 2;
            }
            Omok_play_data.Add(new Vector2(x, y));
            if (Win_check(array[x, y]))
            {
                if(nowColor == myColor)
                {
                    WinText.text = "승리";
                }
                else
                {
                    WinText.text = "패배";
                }
                total1 = date + "," + Omok_play_data.Count.ToString() + "," + myColor;
                login_information_.GetComponent<Login_information>().set_upgrade_omok(Omok_play_data, total1);
                WinPanel.SetActive(true);
            }

            if (nowColor == "black")
            {
                nowColor = "white";
                Turn.text = PhotonNetwork.PlayerList[1].NickName+"의 턴";
            }
            else if (nowColor == "white")
            {
                nowColor = "black";
                Turn.text = PhotonNetwork.PlayerList[0].NickName+"의 턴";
            }
            myTurn = !myTurn;
        }
    }



    private bool startFlag = false;
    private float timer = 0f;
    private void Update()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2 && startFlag == false)
        {
            WaitPanel.SetActive(false);
            startFlag = true;
            Turn.text = PhotonNetwork.PlayerList[0].NickName + "의 턴";
            name1_text_.text = PhotonNetwork.PlayerList[1].NickName;
            name2_text_.text = PhotonNetwork.PlayerList[0].NickName;
        }
        else if(startFlag == false) // 매칭 대기 메시지
        {
            timer += Time.deltaTime;
            if(timer > 0.6f)
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
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.y > 120 && Input.mousePosition.x > 40 && Input.mousePosition.y < 955 && Input.mousePosition.x < 875)
            {
                if (myTurn)
                {
                    Vector2 localCursor1 = new Vector2(Input.mousePosition.x + 427 - 460, Input.mousePosition.y - 138 + 22.25f);
                    int x = (int)(((localCursor1.x) - 26.4 + 44.5 / 2.0) / 44.5);
                    int y = 18 - (int)(((localCursor1.y) - 25.8 + 44.5 / 2.0) / 44.5);
                    double location_x = 26.4 + 44.5 * x - 40 / 2;
                    double location_y = 26.4 + 44.5 * (18 - y) - 40 / 2;
                    PV.RPC("ChatRPC", RpcTarget.All, "<" + myColor + ">" + x + "," + y);

                    // 바둑돌 놓기
                    PV.RPC("OmokPlay", RpcTarget.All, myColor, x, y, location_x, location_y);
                }
            }
        }
    }

    int[,] array = new int[19, 19];
    public bool Win_check(int team)
    {
        for (int i = 0; i < 19; i++)
        {
            for (int h = 0; h < 19; h++)
            {
                if (array[h, i] == team)
                {
                    int temp = 0;
                    if (h < 15)
                    { //가로
                        for (int r = h; r < h + 5; r++)
                        {
                            if (array[r, i] == team)
                            {
                                temp += array[r, i];
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (temp == team * 5)
                        {

                            return true;
                        }
                    }
                    temp = 0;
                    if (i < 15)
                    { //세로
                        for (int r = i; r < i + 5; r++)
                        {
                            if (array[h, r] == team)
                            {
                                temp += array[h, r];
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (temp == team * 5)
                        {

                            return true;
                        }
                    }
                    temp = 0;
                    if (h > 3 && i < 15)
                    { //대각선1 <슬래시모양>
                        int weight = h;
                        int height = i;
                        for (int r = 0; r < 5; r++)
                        {
                            if (array[weight - r, height + r] == team) temp += array[weight - r, height + r];
                            else break;
                        }
                        if (temp == team * 5)
                        {

                            return true;
                        }
                    }
                    temp = 0;
                    if (i < 15 && h < 15)
                    { //대각선2 <역슬래시 모양>
                        int weight = h;
                        int height = i;
                        for (int r = 0; r < 5; r++)
                        {
                            if (array[weight + r, height + r] == team) temp += array[weight + r, height + r];
                            else break;
                        }
                        if (temp == team * 5)
                        {

                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void ToHome()
    {
        // 네트워크 객체 파괴 (연결종료)
        Destroy(NetworkManager.myObj);
        SceneManager.LoadScene("Main");
    }

    public void WaitingBackEvent()
    {
        PhotonNetwork.LeaveRoom();
        Destroy(NetworkManager.myObj);
        SceneManager.LoadScene("Lobby");
    }

}
