using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text StateText;
    public Text LobbyInfoText;
    public Text UserNameText;
    private string gameTitle = "����";

    public GameObject Omok, Chess;

    public Text RoomName;

    public static GameObject myObj;
    public GameObject LoadingPanel;
    // Start is called before the first frame update
    void Start()
    {
        myObj = this.gameObject;
        Connect();
    }

    private bool isJoinLobby = false;
    // Update is called once per frame
    void Update()
    {
        StateText.text = PhotonNetwork.NetworkClientState.ToString();
        if(StateText.text == "JoinedLobby" && isJoinLobby == false) // �ε�
        {
            LoadingPanel.SetActive(false);
            isJoinLobby = true;
        }
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "�κ� / " + PhotonNetwork.CountOfPlayers + "����";
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    //-------------------------------------------
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        GameObject login_information_ = GameObject.Find("Login_");
        string myName = login_information_.GetComponent<Login_information>().return_name();
        PhotonNetwork.LocalPlayer.NickName = myName;
        UserNameText.text = PhotonNetwork.LocalPlayer.NickName;
        //myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();
    //----------------------------------------------


    //------------------------------------------------
    public void CreateRoom() => PhotonNetwork.CreateRoom(RoomName.text == "" ? "Room" + Random.Range(0, 100) + "(" + gameTitle + ")" : RoomName.text + "(" + gameTitle + ")", new RoomOptions { MaxPlayers = 2 });

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();


    public override void OnJoinedRoom()
    {
        DontDestroyOnLoad(this.gameObject);
        if (gameTitle == "����")
        {
            SceneManager.LoadScene("PlayerOmok");
        }
        else if(gameTitle == "ü��")
        {
            SceneManager.LoadScene("PlayerChess");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomName.text = ""; CreateRoom(); }

    public override void OnJoinRandomFailed(short returnCode, string message) { RoomName.text = ""; CreateRoom(); }




    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    // ����ư -2 , ����ư -1 , �� ����
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else {
            gameTitle = myList[multiple + num].Name.Substring(myList[multiple + num].Name.Length - 3, 2);
            PhotonNetwork.JoinRoom(myList[multiple + num].Name); 
            
        }
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // �ִ�������
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // ����, ������ư
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // �������� �´� ����Ʈ ����
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }

    public void ToggleChange()
    {
        if (Omok.GetComponent<Toggle>().isOn)
        {
            gameTitle = "����";
        }else if (Chess.GetComponent<Toggle>().isOn)
        {
            gameTitle = "ü��";
        }
    }

    public void ToHome()
    {
        Destroy(this.gameObject);
        SceneManager.LoadScene("Main");
    }
}
