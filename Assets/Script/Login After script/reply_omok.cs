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

public class reply_omok : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    ArrayList a1a1;
    GameObject login_information_;
	GameObject enter;
	public Toggle to;
	public InputField count_input;
    public Text end_count;
    int ccnt, end_ccnt;

    public GameObject black_dol;
    public GameObject white_dol;
	public GameObject blackimg;

	Vector2 cucu;

	int team = 1;

	Vector2 ai;

	int[,] array = new int[19, 19];

	ArrayList cur_loca;

	bool custom_check = false;

	int replayCount = 1;

    GameObject dol;
	int page;
    void Start()
    {
		blackimg.SetActive(false);
		cucu = new Vector2();
		ai = new Vector2();
        cur_loca = new ArrayList();
        dol = black_dol;
        login_information_ = GameObject.Find("Login_");
		enter = GameObject.Find("entering");
		string anwser = enter.GetComponent<omok_entering>().d_date;
		ArrayList temptemp = login_information_.GetComponent<Login_information>().return_omokdate();
		for (int i=0; i < temptemp.Count; i++)
        {
            if (anwser.Equals(temptemp[i].ToString()))
            {
				page = i;
				break;
            }
        }
		Debug.Log(page);
		GameObject.Find("sel_col_canvas/right").transform.localScale = new Vector2(-1, 1);
        GameObject.Find("Canvas/pre_btn").transform.localScale = new Vector2(-1, 1);
        ccnt = 1;
        ArrayList aa = login_information_.GetComponent<Login_information>().return_omoklist();
        a1a1 = (ArrayList)aa[page]; //���� ���ϴ� ���÷��� ������������
        end_ccnt = a1a1.Count;
        count_input.text = ccnt.ToString();
        end_count.text = "/ " + end_ccnt.ToString();
		dol_graphic();

	}
	public void OnPointerClick(PointerEventData eventData)
    {
		if (custom_check)
		{
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(),
		eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
				return;
			Vector2 localCursor1 = new Vector2(localCursor.x + 427, localCursor.y + 427);
			int x = (int)(((localCursor1.x) - 26.4 + 44.5 / 2.0) / 44.5);
			int y = 18 - (int)(((localCursor1.y) - 25.8 + 44.5 / 2.0) / 44.5);
			double location_x = 26.4 + 44.5 * x - 40 / 2;
			double location_y = 26.4 + 44.5 * (18 - y) - 40 / 2;
			if (array[x, y] == 0)
			{
				RectTransform rectTran;
				rectTran = dol.GetComponent<RectTransform>();
				rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
				rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
				rectTran.GetChild(0).GetComponent<Text>().text = "��";
				double return_x = location_x - 427;
				double return_y = location_y - 427;
				var temp = Instantiate(dol);
				temp.transform.localScale = Vector2.one;
				temp.transform.SetParent(GameObject.Find("Canvas/OmokPan").transform, false);
				temp.transform.localPosition = new Vector2((float)return_x + 20f, (float)return_y + 20f);
				array[x, y] = team;
				cucu = new Vector2(x, y);
				if (team == 1)
				{
					dol = white_dol;
					team = 2;
				}
				else
				{
					dol = black_dol;
					team = 1;
				}
			}
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
	public void left_click()
    {
		replayCount = 1;
		ccnt--;
        if (ccnt < 1) ccnt = 1;
        Debug.Log(ccnt.ToString());
        count_input.text = (ccnt).ToString();
		team = 1;
		dol = black_dol;
		dol_graphic();
	}

    public void right_click()
    {
		replayCount = 1;
		ccnt++;
        if (ccnt > end_ccnt) ccnt = end_ccnt;
        Debug.Log(ccnt.ToString());
        count_input.text = (ccnt).ToString();
		team = 1;
		dol = black_dol;
		dol_graphic();
	}
	public void counter_input()
    {
		ccnt = int.Parse(count_input.text);
		if (ccnt <= 0) { ccnt = 1; count_input.text = (1).ToString(); }
		team = 1;
		dol = black_dol;
		dol_graphic();
	}
	public void dol_graphic()
	{
		GameObject[] temp_dol = GameObject.FindGameObjectsWithTag("temp"); //������ �׸����� �ִ� ����
		for (int i = 0; i < temp_dol.Length; i++)
		{
			if(!(temp_dol[i].name == "BlackStone") && !(temp_dol[i].name == "WhiteStone"))
            {
				Destroy(temp_dol[i]); //�׸� ĭ �� �ı�
			}
		}
		for(int i=0;i< 19; i++)
        {
			for (int h = 0; h < 19; h++)
			{
				array[h, i] = 0;
			}
		}
		for (int i = 0; i < ccnt; i++)
		{
			Vector2 temp1 = (Vector2)a1a1[i];
			int x = (int)temp1.x;
			int y = (int)temp1.y;
			array[x, y] = team;
			cur_loca.Add(temp1);
			double location_x = 26.4 + 44.5 * x - 40 / 2;
			double location_y = 26.4 + 44.5 * (18 - y) - 40 / 2;
			RectTransform rectTran;
			rectTran = dol.GetComponent<RectTransform>();
			rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
			rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
			rectTran.GetChild(0).GetComponent<Text>().text = "";
			double return_x = location_x - 427;
			double return_y = location_y - 427;
			var temp = Instantiate(dol);
			temp.transform.localScale = Vector2.one;
			temp.transform.SetParent(GameObject.Find("Canvas/OmokPan").transform, false);
			temp.transform.localPosition = new Vector2((float)return_x + 20f, (float)return_y + 20f);
			if (dol.name.Contains("Black"))
			{
				dol = white_dol;
				team = 2; //��
			}
			else
			{
				dol = black_dol;
				team = 1; //��
			}
		}
		cucu = (Vector2)cur_loca[cur_loca.Count - 1];
	}
    public void exit_back()
    {
		Destroy(GameObject.Find("entering"));
        SceneManager.LoadScene("SelectOmok");
    }

    public void next_btn()
    {
        if (dol.name.Contains("White"))
        {
			double location_x, location_y, return_x, return_y;
			RectTransform rectTran1;
			int total = heuristic(3, true, cucu, -99999999, 99999999); //�޸���ƽ depth = 3
			rectTran1 = dol.GetComponent<RectTransform>();
			rectTran1.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
			rectTran1.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
			rectTran1.GetChild(0).GetComponent<Text>().text = replayCount++ + "";

			location_x = 26.4 + 44.5 * ai.x - 40 / 2;
			location_y = 26.4 + 44.5 * (18 - ai.y) - 40 / 2;
			return_x = location_x - 427;
			return_y = location_y - 427;
			var temp1 = Instantiate(dol);
			temp1.transform.localScale = Vector2.one;
			temp1.transform.SetParent(GameObject.Find("Canvas/OmokPan").transform, false);
			temp1.transform.localPosition = new Vector2((float)return_x + 20f, (float)return_y + 20f);
			Debug.Log("AI�� ��ǥ�� " + ai.x + " , " + ai.y);
			cucu = ai;
			array[(int)ai.x, (int)ai.y] = 2;
			cur_loca.Add(ai);
			if (Win_check(2))
			{
				Debug.Log("�Է�");
			}
			dol = black_dol;
			team = 1;
        }
        else
        {
			double location_x, location_y, return_x, return_y;
			RectTransform rectTran1;
			int total = heuristic(3, true, cucu, -99999999, 99999999); //�޸���ƽ depth = 3
			rectTran1 = dol.GetComponent<RectTransform>();
			rectTran1.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
			rectTran1.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
			rectTran1.GetChild(0).GetComponent<Text>().text = replayCount++ + "";

			location_x = 26.4 + 44.5 * ai.x - 40 / 2;
			location_y = 26.4 + 44.5 * (18 - ai.y) - 40 / 2;
			return_x = location_x - 427;
			return_y = location_y - 427;
			var temp1 = Instantiate(dol);
			temp1.transform.localScale = Vector2.one;
			temp1.transform.SetParent(GameObject.Find("Canvas/OmokPan").transform, false);
			temp1.transform.localPosition = new Vector2((float)return_x + 20f, (float)return_y + 20f);
			Debug.Log("AI�� ��ǥ�� " + ai.x + " , " + ai.y);
			cucu = ai;
			array[(int)ai.x, (int)ai.y] = 1;
			cur_loca.Add(ai);
			if (Win_check(1))
			{
				Debug.Log("�Է�");
			}
			dol = white_dol;
			team = 2;
		}
    }

    public void pre_btn()
    {
		if(cur_loca.Count > 0)
        {
			replayCount--;
			GameObject[] dols = GameObject.FindGameObjectsWithTag("temp");
			Vector2 re_move = (Vector2)cur_loca[cur_loca.Count - 1];
			double location_x = 26.4 + 44.5 * re_move.x - 40 / 2;
			double location_y = 26.4 + 44.5 * (18 - re_move.y) - 40 / 2;
			double return_x = location_x - 427;
			double return_y = location_y - 427;
			re_move = new Vector2((float)return_x + 20f, (float)return_y + 20f);
			Debug.Log("������ ��ǥ : " + re_move.x + " , " + re_move.y);
			for (int i = 0; i < dols.Length; i++)
			{
				Debug.Log("���� ��ǥ : " + dols[i].transform.localPosition.x + " , " + dols[i].transform.localPosition.y);
				if ((dols[i].transform.localPosition.x.Equals(re_move.x)) && (dols[i].transform.localPosition.y.Equals(re_move.y)))
				{
					Destroy(dols[i]);
					break;
				}
			}
			re_move = (Vector2)cur_loca[cur_loca.Count - 1];
			array[(int)re_move.x, (int)re_move.y] = 0;
			cur_loca.RemoveAt(cur_loca.Count - 1);
			cucu = (Vector2)cur_loca[cur_loca.Count - 1];
			if(cur_loca.Count % 2 == 0)
            {
				dol = black_dol;
				team = 1;
            }
            else{
				dol = white_dol;
				team = 2;
			}
		}
	}

	public int heuristic(int depth, bool turn, Vector2 stone, int alpha, int beta)
	{ //�޸���ƽ ����
	  ///////////////////////////////////////////////������ִ�ĭ Ȯ��
		bool full = false;
		int x_x = 0, y_y = 0;
		for (int i = 0; i < 18 * 18; i++)
		{
			if (x_x == 19)
			{
				x_x = 0;
				y_y++;
			}
			if (array[x_x, y_y] == 0)
			{
				full = false;
				break;
			}
			else
			{
				full = true;
			}
			x_x++;
		}
		if (full)
		{
			return 0;
		}
		///////////////////////////////////////////////
		Vector2 nextStone = new Vector2();
		int team1 = 0, team2 = 0;
		if (turn)
		{ team1 = team;
			if (team == 1)
			{
				team2 = 2;
			}
			else
			{
				team2 = 1;
			}
		}
		else
		{
			team2 = team;
			if (team == 1)
            {
				team1 = 2;
			}
            else
            {
				team1 = 1;
            }
		}
		if (Win_check(team2))
		{
			return 1000000;
		}
		else if (depth == 0)
		{ //������ ���� �پ��� ����
			int b = score(turn, stone); //���� ��� => turn = ���
			if (b > 18039)
			{ //����� �Ѷ� ������ ����ġ ���� ũ�� ���
			  //ù������ ��������� �������� first_play ����
				return b;
			}
			else
			{ //�ƴϸ� ����
				return score(!turn, stone);
			}
		}
		if (turn)
		{ //AI
			int temp = -99999999;
			bool check_cut = false;
			for (int i = 0; i < 19; i++)
			{
				for (int h = 0; h < 19; h++)
				{
					if (array[h, i] == 0)
					{
						array[h, i] = team1;
						nextStone = new Vector2(h, i);
						int rere_value = heuristic(depth - 1, false, nextStone, alpha, beta);
						if (rere_value > temp)
						{ //�������� �̹� �޸���ƽ ���� ũ��
							temp = rere_value;
							ai.x = h;
							ai.y = i;
						}
						alpha = Mathf.Max(temp, alpha); //���� �� ã��
						array[h, i] = 0;
						if (beta <= alpha)
						{ //���ĺ�Ÿ ����ġ��
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
		else
		{ //�ּ����� <���>
			int temp = 99999999;
			bool check_cut = false;
			for (int i = 0; i < 19; i++)
			{
				for (int h = 0; h < 19; h++)
				{
					if (array[h, i] == 0)
					{
						array[h, i] = team1;
						nextStone = new Vector2(h, i);
						int rere_value = heuristic(depth - 1, true, nextStone, alpha, beta);
						rere_value *= -1;
						temp = Mathf.Min(rere_value, temp);
						beta = Mathf.Min(temp, beta);
						array[h, i] = 0;
						if (beta <= alpha)
						{
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
	}

	public int score(bool turn, Vector2 loca)
	{ //���� ���
		int team1 = 1;
		int temp = 0; //������		
		int x_score = (int)loca.x, y_score = (int)loca.y; //curloca
		Vector2 end1 = new Vector2(loca.x, loca.y), end2 = new Vector2(loca.x, loca.y); //����ϴ� �ٵ��� ���� �� ��ǥ
        if (turn)
        {
			team1 = team;
		}
        else
		{
			if (team == 1)
			{
				team1 = 2;
			}
			else
			{
				team1 = 1;
			}
		}
		int cnt = 1, addvalue = 1;
		bool twist = true;
		while (true)
		{ //���� => �̾����ִ� ���� ���� ��������
			if (x_score + addvalue > 18)
			{
				end1 = new Vector2(x_score + (addvalue - 1), y_score);
				twist = false;
				addvalue = -1;
			}
			if (x_score + addvalue < 0)
			{
				end2 = new Vector2(x_score + (addvalue + 1), y_score);
				break;
			}
			if (twist)
			{
				if (array[x_score + addvalue, y_score] == team1)
				{
					cnt++;
					addvalue++;
				}
				else
				{
					end1 = new Vector2(x_score + (addvalue - 1), y_score);
					twist = false;
					addvalue = -1;
				}
			}
			else
			{
				if (array[x_score + addvalue, y_score] == team1)
				{
					cnt++;
					addvalue--;
				}
				else
				{
					end2 = new Vector2(x_score + (addvalue + 1), y_score);
					break;
				}
			}
		}
		int andscore = 0;
		if (cnt == 1)
		{
			andscore = 0;
		}
		else if (cnt == 2)
		{
			andscore = 1900;
		}
		else if (cnt == 3)
		{
			andscore = 9001;
		}
		else if (cnt == 4)
		{
			andscore = 200000;
		}
		else if (cnt == 5)
		{
			return 1000000;
		}
		bool sidecheck1 = sideecheck((int)end1.x, (int)end1.y, 1), sidecheck2 = sideecheck((int)end2.x, (int)end2.y, 1);
		if ((sidecheck1 == false && sidecheck2) || (sidecheck1 && sidecheck2 == false))
		{
			if ((end2.x != 18 || end2.x != 0))
			{ //������ �����ִµ� ���� ���� �ƴ� <������ ���� ��>
				andscore += 25;
			}
			else
			{ //<0�� 15�� ��輱>
				andscore += 14;
			}
		}
		else if (sidecheck1 && sidecheck2)
		{ //���� ����
			andscore += 40;
		}
		else
		{ //������ �����ִ°� 
			andscore = 0;
		}
		temp += andscore;
		///////////////////////////////////////////////////// /////////////////////////
		cnt = 1;
		addvalue = 1;
		twist = true;
		while (true)
		{ //����  => �̾����ִ� ���� ���� ��������
			if (y_score + addvalue > 18)
			{
				end1 = new Vector2(x_score, y_score + (addvalue - 1));
				twist = false;
				addvalue = -1;
			}
			if (y_score + addvalue < 0)
			{
				end2 = new Vector2(x_score, y_score + (addvalue + 1));
				break;
			}
			if (twist)
			{
				if (array[x_score, y_score + addvalue] == team1)
				{
					cnt++;
					addvalue++;
				}
				else
				{
					end1 = new Vector2(x_score, y_score + (addvalue - 1));
					twist = false;
					addvalue = -1;
				}
			}
			else
			{
				if (array[x_score, y_score + addvalue] == team1)
				{
					cnt++;
					addvalue--;
				}
				else
				{
					end2 = new Vector2(x_score, y_score + (addvalue + 1));
					break;
				}
			}
		}
		andscore = 0;
		if (cnt == 1)
		{
			andscore = 0;
		}
		else if (cnt == 2)
		{
			andscore = 1900;
		}
		else if (cnt == 3)
		{
			andscore = 9001;
		}
		else if (cnt == 4)
		{
			andscore = 200000;
		}
		else if (cnt == 5)
		{
			return 1000000;
		}
		sidecheck1 = sideecheck((int)end1.x, (int)end1.y, 2);
		sidecheck2 = sideecheck((int)end2.x, (int)end2.y, 2);
		if ((sidecheck1 == false && sidecheck2) || (sidecheck1 && sidecheck2 == false))
		{
			if ((end1.y != 18 || end1.y != 0))
			{
				andscore += 25;
			}
			else
			{
				andscore += 14;
			}
		}
		else if (sidecheck1 && sidecheck2)
		{ //���� ����
			andscore += 40;
		}
		else
		{
			andscore = 0;
		}
		temp += andscore;
		//////////////////////////////////////////////////////////////////////////////
		cnt = 1;
		addvalue = 1;
		int addvalue2 = 1;
		twist = true;
		while (true)
		{ //�밢��1 (�������� ���)  => �̾����ִ� ���� ���� ��������
			if (y_score + addvalue > 18 || x_score + addvalue2 > 18)
			{
				end1 = new Vector2(x_score + (addvalue2 - 1), y_score + (addvalue - 1));
				twist = false;
				addvalue = -1;
				addvalue2 = -1;
			}
			if (y_score + addvalue < 0 || x_score + addvalue2 < 0)
			{
				end2 = new Vector2(x_score + (addvalue2 + 1), y_score + (addvalue + 1));
				break;
			}
			if (twist)
			{
				if (array[x_score + addvalue2, y_score + addvalue] == team1)
				{
					cnt++;
					addvalue++;
					addvalue2++;
				}
				else
				{
					end1 = new Vector2(x_score + (addvalue2 - 1), y_score + (addvalue - 1));
					twist = false;
					addvalue = -1;
					addvalue2 = -1;
				}
			}
			else
			{
				if (array[x_score + addvalue2, y_score + addvalue] == team1)
				{
					cnt++;
					addvalue--;
					addvalue2--;
				}
				else
				{
					end2 = new Vector2(x_score + (addvalue2 + 1), y_score + (addvalue + 1));
					break;
				}
			}
		}
		andscore = 0;
		if (cnt == 1)
		{
			andscore = 0;
		}
		else if (cnt == 2)
		{
			andscore = 1900;
		}
		else if (cnt == 3)
		{
			andscore = 9001;
		}
		else if (cnt == 4)
		{
			andscore = 200000;
		}
		else if (cnt == 5)
		{
			return 1000000;
		}
		sidecheck1 = sideecheck((int)end1.x, (int)end1.y, 3);
		sidecheck2 = sideecheck((int)end2.x, (int)end2.y, 3);
		if ((sidecheck1 == false && sidecheck2) || (sidecheck1 && sidecheck2 == false))
		{
			if (((end2.x != 18 || end2.y != 18) || (end2.x != 0 || end2.y != 0)))
			{
				andscore += 25;
			}
			else
			{
				andscore += 14;
			}
		}
		else if (sidecheck1 && sidecheck2)
		{ //���� ����
			andscore += 40;
		}
		else
		{
			andscore = 0;
		}
		temp += andscore;
		//////////////////////////////////////////////////////////////////////////////
		cnt = 1;
		addvalue = 1;
		addvalue2 = -1;
		twist = true;
		while (true)
		{ //�밢��2 (�����ø��)  => �̾����ִ� ���� ���� ��������
			if (y_score + addvalue > 18 || x_score + addvalue2 < 0)
			{
				end1 = new Vector2(x_score + (addvalue2 + 1), y_score + (addvalue - 1));
				twist = false;
				addvalue = -1;
				addvalue2 = 1;
			}
			if (y_score + addvalue < 0 || x_score + addvalue2 > 18)
			{
				end2 = new Vector2(x_score + (addvalue2 - 1), y_score + (addvalue + 1));
				break;
			}
			if (twist)
			{
				if (array[x_score + addvalue2, y_score + addvalue] == team1)
				{
					cnt++;
					addvalue++;
					addvalue2--;
				}
				else
				{
					end1 = new Vector2(x_score + (addvalue2 + 1), y_score + (addvalue - 1));
					twist = false;
					addvalue = -1;
					addvalue2 = 1;
				}
			}
			else
			{
				if (array[x_score + addvalue2, y_score + addvalue] == team1)
				{
					cnt++;
					addvalue--;
					addvalue2++;
				}
				else
				{
					end2 = new Vector2(x_score + (addvalue2 - 1), y_score + (addvalue + 1));
					break;
				}
			}
		}
		andscore = 0;
		if (cnt == 1)
		{
			andscore = 0;
		}
		else if (cnt == 2)
		{
			andscore = 1900;
		}
		else if (cnt == 3)
		{
			andscore = 9001;
		}
		else if (cnt == 4)
		{
			andscore = 200000;
		}
		else if (cnt == 5)
		{
			return 1000000;
		}
		sidecheck1 = sideecheck((int)end1.x, (int)end1.y, 4);
		sidecheck2 = sideecheck((int)end2.x, (int)end2.y, 4);
		if ((!sidecheck1 && sidecheck2) || (sidecheck1 && !sidecheck2))
		{ //���ʸ� ����
			if ((end1.x != 18 || end1.y != 0) || (end1.x != 0 || end1.y != 18))
			{
				andscore += 25;
			}
			else
			{
				andscore += 14;
			}
		}
		else if (sidecheck1 && sidecheck2)
		{ //���� ����
			andscore += 40;
		}
		else
		{
			andscore = 0;
		}
		temp += andscore;
		return temp;
	}

	public bool sideecheck(int x, int y, int direction)
	{ //���� �����ִ��� üũ
		if (direction == 1)
		{ //����
			if (x >= 18 || x <= 0)
			{
				return false;
			}
			else
			{
				if (array[x + 1, y] == 0 || array[x - 1, y] == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		else if (direction == 2)
		{ //����
			if (y >= 18 || y <= 0)
			{
				return false;
			}
			else
			{
				if (array[x, y + 1] == 0 || array[x, y - 1] == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		else if (direction == 3)
		{ //�밢��1
			if ((x >= 18 || y >= 18) || (x <= 0 || y <= 0))
			{
				return false;
			}
			else
			{
				if (array[x + 1, y + 1] == 0 || array[x - 1, y - 1] == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		else if (direction == 4)
		{ //�밢��2
			if ((x >= 18 || y <= 0) || (x <= 0 || y >= 18))
			{
				return false;
			}
			else
			{
				if (array[x + 1, y - 1] == 0 || array[x - 1, y + 1] == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		return false;
	}
	public bool Win_check(int team1)
	{
		for (int i = 0; i < 19; i++)
		{
			for (int h = 0; h < 19; h++)
			{
				if (array[h, i] == team1)
				{
					int temp = 0;
					if (h < 15)
					{ //����
						for (int r = h; r < h + 5; r++)
						{
							if (array[r, i] == team1)
							{
								temp += array[r, i];
							}
							else
							{
								break;
							}
						}
						if (temp == team1 * 5)
						{
							return true;
						}
					}
					temp = 0;
					if (i < 15)
					{ //����
						for (int r = i; r < i + 5; r++)
						{
							if (array[h, r] == team1)
							{
								temp += array[h, r];
							}
							else
							{
								break;
							}
						}
						if (temp == team1 * 5)
						{
							return true;
						}
					}
					temp = 0;
					if (h > 3 && i < 15)
					{ //�밢��1 <�����ø��>
						int weight = h;
						int height = i;
						for (int r = 0; r < 5; r++)
						{
							if (array[weight - r, height + r] == team1) temp += array[weight - r, height + r];
							else break;
						}
						if (temp == team1 * 5)
						{
							return true;
						}
					}
					temp = 0;
					if (i < 15 && h < 15)
					{ //�밢��2 <�������� ���>
						int weight = h;
						int height = i;
						for (int r = 0; r < 5; r++)
						{
							if (array[weight + r, height + r] == team1) temp += array[weight + r, height + r];
							else break;
						}
						if (temp == team1 * 5)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Update is called once per frame
	void Update()
    {
	}
}
