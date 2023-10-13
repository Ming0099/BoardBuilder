using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Unity;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using Firebase.Firestore;

public class Login_information : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    public InputField email;
    public InputField pw;
    public InputField nameField;
    public Text warning;
    private DatabaseReference databaseReference;
    FirebaseFirestore db;
    string name11 = "";

    public List<string> chess_user_team;

    public ArrayList omok_date;
    public ArrayList chess_date;

    public ArrayList omoklists, chesslists;

    string uid;

    public static Login_information instance = null;
    User user_info;
    class User
    {
        public string name;
        public int omok_cnt;
        public int chess_cnt;
        public User()
        {
            name = "";
            omok_cnt = 0;
            chess_cnt = 0;
        }
        public User(string user_name)
        {
            name = user_name;
            omok_cnt = 0;
            chess_cnt = 0;
        }
    }

    void Start()
    {
        chess_user_team = new List<string>();
        omok_date = new ArrayList();
        chess_date = new ArrayList();
        omoklists = new ArrayList();
        chesslists = new ArrayList();
        FirebaseDatabase.GetInstance("https://boardgame-login-default-rtdb.firebaseio.com/");
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }

    void Awake()
    {
    }

    public void Create()
    {
        if (email.text.Trim().Length > 0 && pw.text.Trim().Length >= 6 && nameField.text.Trim().Length > 2)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email.text.Trim(), pw.text.Trim()).ContinueWithOnMainThread(task => {
                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    AuthResult newUser = task.Result;
                    FirebaseUser newUser1 = auth.CurrentUser;
                    name11 = nameField.text.Trim();
                    add(newUser1.UserId, name11);
                    warning.text = "";
                    SceneManager.LoadScene("SampleScene");
                }
                else
                {
                    Debug.LogError("회원가입 취소");
                }
            });
        }
        else
        {
            warning.text = "입력이 잘못 되어었습니다.";
        }
    }

    public void add(string email1, string name1)
    {
        User us = new User(name1);
        string jsonData = JsonUtility.ToJson(us);
        databaseReference.Child(email1).SetRawJsonValueAsync(jsonData);
    }

    public void set_upgrade_omok(ArrayList play, string zero)
    {
        user_info.omok_cnt += 1;
        string[] rr = zero.Split(',');
        if (omok_date.Count < 10)
        {
            omok_date.Add(rr[0]);
            omoklists.Add(play);
        }
        else
        {
            omok_date[(user_info.omok_cnt - 1) % 10] = rr[0];
            omoklists[(user_info.omok_cnt - 1) % 10] = play;
        }
        int enter_num = (user_info.omok_cnt % 10);
        if (enter_num == 0) { enter_num = 10; }
        string path = "omok/Board" + (enter_num).ToString();
        if (user_info.omok_cnt > 10)
        {
            databaseReference.Child(uid).Child(path).RemoveValueAsync();
        }
        string path1 = "Board" + (enter_num).ToString();
        int cnt = 1;
        databaseReference.Child(uid).Child("omok").Child(path1).Child("0").SetValueAsync(zero);
        foreach (Vector2 r in play)
        {
            int x = (int)r.x;
            int y = (int)r.y;
            string str = x.ToString() + "," + y.ToString();
            databaseReference.Child(uid).Child("omok").Child(path1).Child(cnt.ToString()).SetValueAsync(str);
            cnt++;
        }
        Dictionary<string, object> updates = new Dictionary<string, object> {
            { "omok_cnt", user_info.omok_cnt }
        };
        databaseReference.Child(uid).UpdateChildrenAsync(updates);
    }

    public void set_upgrade_chess(ArrayList pre_loca, ArrayList next_loca, string player1, string date)
    {
        user_info.chess_cnt += 1;
        int enter_num = (user_info.chess_cnt % 10);
        if (enter_num == 0) { enter_num = 10; }
        string path = "chess/Board" + (enter_num).ToString();
        ArrayList total = new ArrayList();
        total.Add(pre_loca);
        total.Add(next_loca);
        if (chess_date.Count <= 10)
        {
            chess_user_team.Add(player1);
            chess_date.Add(date);
            chesslists.Add(total);
        }
        else
        {
            chess_user_team[(user_info.chess_cnt - 1) % 10] = player1;
            chess_date[(user_info.chess_cnt - 1) % 10] = date;
            chesslists[(user_info.chess_cnt - 1) % 10] = total;
        }
        if (user_info.chess_cnt > 10)
        {
            databaseReference.Child(uid).Child(path).RemoveValueAsync();
        }
        int cnt = 1;
        string path1 = "Board" + (enter_num).ToString();
        string zero = date + "," + pre_loca.Count.ToString() + "," + player1;
        databaseReference.Child(uid).Child("chess").Child(path1).Child("0").SetValueAsync(zero);
        foreach (Vector2 r in pre_loca)
        {
            Vector2 next = (Vector2)next_loca[cnt - 1];
            int prex = (int)r.x;
            int prey = (int)r.y;
            int nextx = (int)next.x;
            int nexty = (int)next.y;
            string str = prex.ToString() + "," + prey.ToString() + ":" + nextx.ToString() + "," + nexty.ToString();
            databaseReference.Child(uid).Child("chess").Child(path1).Child(cnt.ToString()).SetValueAsync(str);
            cnt++;
        }
        Dictionary<string, object> updates = new Dictionary<string, object> {
            { "chess_cnt", user_info.chess_cnt }
        };
        databaseReference.Child(uid).UpdateChildrenAsync(updates);
    }

    public void Login()
    {
        if (email.text.Trim().Length > 0 && pw.text.Trim().Length >= 6)
        {
            auth.SignInWithEmailAndPasswordAsync(email.text.Trim(), pw.text.Trim()).ContinueWithOnMainThread(task1 => {
                if (task1.IsCompleted && !task1.IsCanceled && !task1.IsFaulted)
                {
                    AuthResult newUser = task1.Result;
                    FirebaseUser newUser1 = auth.CurrentUser;
                    uid = newUser1.UserId;
                    warning.text = "";
                    databaseReference.Child(newUser1.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled)
                        {
                            Debug.LogError("로드 취소");
                        }
                        else if (task.IsFaulted)
                        {
                            Debug.LogError("로드 실패");
                        }
                        else //로그온되면
                        {
                            Debug.Log("로그인 완료");
                            user_info = new User();
                            DataSnapshot snapshot = task.Result;
                            name11 = snapshot.Child("name").Value.ToString();
                            user_info.name = name11;
                            user_info.omok_cnt = int.Parse(snapshot.Child("omok_cnt").Value.ToString());
                            user_info.chess_cnt = int.Parse(snapshot.Child("chess_cnt").Value.ToString());
                            if (instance == null)
                            {
                                instance = this;
                            }
                            else if (instance != this && this.name == "Login_")
                            {
                                Destroy(gameObject);
                            }
                            DontDestroyOnLoad(gameObject);
                            Destroy(GameObject.Find("Login_file"));
                            ArrayList temp_loca_omok = new ArrayList();
                            if (user_info.omok_cnt > 10) //오목
                            {
                                for (int i = 1; i <= 10; i++)
                                {
                                    string ss = "Board" + i.ToString();
                                    var lo_ca = snapshot.Child("omok").Child(ss);
                                    string[] arr = lo_ca.Child("0").Value.ToString().Split(',');
                                    omok_date.Add(arr[0]);
                                    for (int k = 1; k <= int.Parse(arr[1]); k++)
                                    {
                                        string str1 = lo_ca.Child(k.ToString()).Value.ToString();
                                        int x = int.Parse(str1.Substring(0, str1.IndexOf(",")).Trim());
                                        int y = int.Parse(str1.Substring(str1.IndexOf(",") + 1).Trim());
                                        temp_loca_omok.Add(new Vector2(x, y));
                                    }
                                    omoklists.Add(temp_loca_omok.Clone());
                                    temp_loca_omok.Clear();
                                }
                            }
                            else
                            {
                                for (int i = 1; i < user_info.omok_cnt + 1; i++)
                                {
                                    string ss = "Board" + i.ToString();
                                    var lo_ca = snapshot.Child("omok").Child(ss);
                                    string[] arr = lo_ca.Child("0").Value.ToString().Split(',');
                                    omok_date.Add(arr[0]);
                                    for (int k = 1; k <= int.Parse(arr[1]); k++)
                                    {
                                        string str1 = lo_ca.Child(k.ToString()).Value.ToString();
                                        int x = int.Parse(str1.Substring(0, str1.IndexOf(",")).Trim());
                                        int y = int.Parse(str1.Substring(str1.IndexOf(",") + 1).Trim());
                                        temp_loca_omok.Add(new Vector2(x, y));
                                    }
                                    omoklists.Add(temp_loca_omok.Clone());
                                    temp_loca_omok.Clear();
                                }
                            }
                            ArrayList temp_loca_chess_pre = new ArrayList(); //체스
                            ArrayList temp_loca_chess_next = new ArrayList();
                            if (user_info.chess_cnt >= 10)
                            {
                                for (int i = 1; i <= 10; i++)
                                {
                                    string ss = "Board" + i.ToString();
                                    var lo_ca = snapshot.Child("chess").Child(ss);
                                    string[] arr = lo_ca.Child("0").Value.ToString().Split(',');
                                    chess_user_team.Add(arr[2]);
                                    chess_date.Add(arr[0]);
                                    for (int k = 1; k <= int.Parse(arr[1]); k++)
                                    {
                                        string str1 = lo_ca.Child(k.ToString()).Value.ToString();
                                        string[] sp = str1.Split(':');
                                        int prex = int.Parse(sp[0].Substring(0, sp[0].IndexOf(",")).Trim());
                                        int prey = int.Parse(sp[0].Substring(sp[0].IndexOf(",") + 1).Trim());
                                        int nextx = int.Parse(sp[1].Substring(0, sp[1].IndexOf(",")).Trim());
                                        int nexty = int.Parse(sp[1].Substring(sp[1].IndexOf(",") + 1).Trim());
                                        temp_loca_chess_pre.Add(new Vector2(prex, prey));
                                        temp_loca_chess_next.Add(new Vector2(nextx, nexty));
                                    }
                                    ArrayList pre_next = new ArrayList();
                                    pre_next.Add(temp_loca_chess_pre.Clone());
                                    pre_next.Add(temp_loca_chess_next.Clone());
                                    chesslists.Add(pre_next.Clone());
                                    temp_loca_chess_pre.Clear();
                                    temp_loca_chess_next.Clear();
                                }
                            }
                            else
                            {
                                for (int i = 1; i < user_info.chess_cnt + 1; i++)
                                {
                                    string ss = "Board" + i.ToString();
                                    var lo_ca = snapshot.Child("chess").Child(ss);
                                    string[] arr = lo_ca.Child("0").Value.ToString().Split(',');
                                    chess_user_team.Add(arr[2]);
                                    chess_date.Add(arr[0]);
                                    for (int k = 1; k <= int.Parse(arr[1]); k++)
                                    {
                                        string str1 = lo_ca.Child(k.ToString()).Value.ToString();
                                        string[] sp = str1.Split(':');
                                        int prex = int.Parse(sp[0].Substring(0, sp[0].IndexOf(",")).Trim());
                                        int prey = int.Parse(sp[0].Substring(sp[0].IndexOf(",") + 1).Trim());
                                        int nextx = int.Parse(sp[1].Substring(0, sp[1].IndexOf(",")).Trim());
                                        int nexty = int.Parse(sp[1].Substring(sp[1].IndexOf(",") + 1).Trim());
                                        temp_loca_chess_pre.Add(new Vector2(prex, prey));
                                        temp_loca_chess_next.Add(new Vector2(nextx, nexty));
                                    }
                                    ArrayList pre_next = new ArrayList();
                                    pre_next.Add(temp_loca_chess_pre.Clone());
                                    pre_next.Add(temp_loca_chess_next.Clone());
                                    chesslists.Add(pre_next.Clone());
                                    temp_loca_chess_pre.Clear();
                                    temp_loca_chess_next.Clear();
                                }
                            }

                            SceneManager.LoadScene("Main");
                        }
                    });
                }
                else if (!task1.IsFaulted)
                {
                    Debug.Log("로그인 실패");
                    warning.text = "입력이 잘못 되어었습니다.";
                }
                else
                {
                    Debug.Log("로그인 취소");
                }
            });
        }
        else
        {
            warning.text = "입력이 잘못 되어었습니다.";
        }
    }
    public string return_name()
    {
        return name11;
    }
    public ArrayList return_omoklist()
    {
        return omoklists;
    }
    public ArrayList return_chesslist()
    {
        return chesslists;
    }
    public ArrayList return_omokdate()
    {
        return omok_date;
    }
    public ArrayList return_chessdate()
    {
        return chess_date;
    }
    public void Log_out()
    {
        auth.SignOut();
    }


}