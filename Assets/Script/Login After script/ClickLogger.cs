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
public class ClickLogger : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	GameObject login_information_;
	public ArrayList Omok_play_data;

	DateTime t;
	string date;
	string total1;

	public GameObject sel_col_Canvas;
	public GameObject back_gorund;
	public GameObject black_dol;
	public GameObject white_dol;
	public GameObject Game_End_Canvas;
	public GameObject AIColor_Black, AIColor_White, PlayerColor_Black, PlayerColor_White;
	public Text End_Text;

    GameObject AI;
	GameObject PLAYER;

	int[,] array = new int[19, 19];
	bool okay_click = false;
	bool first_play = true;
	int score1, score2, score3, score4;
	int pre_x, pre_y;
	public Toggle White_toggle, Black_toggle;
	Vector2 ai = new Vector2();
	bool check1 = false;
	int person_x, person_y;
	void Start()
	{
		t = DateTime.Now;
		date = t.ToString("yyyy/MM/dd HH:mm:ss");
		login_information_ = GameObject.Find("Login_");
		score1 = 25;
		score2 = 14;
		score3 = 40;
		score4 = -10;
        Game_End_Canvas.SetActive(false);
		AIColor_Black.SetActive(false);
		AIColor_White.SetActive(false);
		PlayerColor_Black.SetActive(false);
		PlayerColor_White.SetActive(false);
    }
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
	public void OnPointerDown(PointerEventData eventData)
	{
		var obj = GameObject.Find("Canvas/OmokPan");
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(),
		eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
			return;
		Vector2 localCursor1 = new Vector2(localCursor.x + 427, localCursor.y + 427);
		int x = (int)(((localCursor1.x) - 26.4 + 44.5 / 2.0) / 44.5);
		int y = 18 - (int)(((localCursor1.y) - 25.8 + 44.5 / 2.0) / 44.5);
		double location_x = 26.4 + 44.5 * x - 40 / 2;
		double location_y = 26.4 + 44.5 * (18 - y) - 40 / 2;
		Debug.Log("LocalCursor:" + localCursor1 + "\n 좌표는 " + x + " " + y);
        if (array[x, y] == 0)
		{
			RectTransform rectTran;
			rectTran = PLAYER.GetComponent<RectTransform>();
			rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
			rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
			double return_x = location_x - 427;
			double return_y = location_y - 427;
			var temp = Instantiate(PLAYER);
			temp.transform.localScale = Vector2.one;
			temp.transform.SetParent(GameObject.Find("Canvas/OmokPan").transform, false);
			temp.transform.localPosition = new Vector2((float)return_x + 20f, (float)return_y + 20f);
			array[x, y] = 1;
			Omok_play_data.Add(new Vector2(x, y)); //데이터 저장
			if (Win_check(1))
			{
                back_gorund.SetActive(true);
                Game_End_Canvas.SetActive(true);
                End_Text.text = "승리";
                total1 = date + "," + Omok_play_data.Count.ToString() + "," + PLAYER.name;
				login_information_.GetComponent<Login_information>().set_upgrade_omok(Omok_play_data, total1);
			}
			check1 = true;
			person_x = x;
			person_y = y;
		}
	}
	public void OnPointerUp(PointerEventData eventData)
	{
        if (check1)
		{
			double location_x, location_y, return_x, return_y;
			RectTransform rectTran1;
			int total = heuristic(3, true, new Vector2(person_x, person_y), -99999999, 99999999); //휴리스틱 depth = 3
			rectTran1 = AI.GetComponent<RectTransform>();
			rectTran1.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
			rectTran1.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);

			location_x = 26.4 + 44.5 * ai.x - 40 / 2;
			location_y = 26.4 + 44.5 * (18 - ai.y) - 40 / 2;
			return_x = location_x - 427;
			return_y = location_y - 427;
			var temp1 = Instantiate(AI);
			temp1.transform.localScale = Vector2.one;
			temp1.transform.SetParent(GameObject.Find("Canvas/OmokPan").transform, false);
			temp1.transform.localPosition = new Vector2((float)return_x + 20f, (float)return_y + 20f);
			Debug.Log("AI의 좌표는 " + ai.x + " , " + ai.y);
			array[(int)ai.x, (int)ai.y] = 2;
			Omok_play_data.Add(new Vector2(ai.x, ai.y)); //데이터 저장
			first_play = true;
            if (Win_check(2))
			{
                back_gorund.SetActive(true);
                Game_End_Canvas.SetActive(true);
                End_Text.text = "패배";
                total1 = date + "," + Omok_play_data.Count.ToString() + "," + PLAYER.name;
				login_information_.GetComponent<Login_information>().set_upgrade_omok(Omok_play_data, total1);
			}
			check1 = false;
        }
	}

	public int heuristic(int depth, bool turn, Vector2 stone, int alpha, int beta)
	{ //휴리스틱 동작
	  ///////////////////////////////////////////////비워져있는칸 확인
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
		int team = 0;
		if (turn)
		{ team = 1; }
		else
		{
			team = 2;
		}
		if (Win_check(team))
		{
			return 1000000;
		}
		else if (depth == 0)
		{ //위에서 부터 줄어드는 구조
			int b = score(turn, stone); //점수 계산 => turn = 사람
			if (b > 18039 || first_play == false)
			{ //사람이 둘때 점수가 조건치 보다 크면 방어
			  //첫수에는 방어적으로 가기위해 first_play 설정
				return b;
			}
			else
			{ //아니면 공격
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
						array[h, i] = 2;
						pre_x = (int)stone.x;
						pre_y = (int)stone.y;
						nextStone = new Vector2(h, i);
						int rere_value = heuristic(depth - 1, false, nextStone, alpha, beta);
						if (rere_value > temp)
						{ //전값보다 이번 휴리스틱 값이 크면
							temp = rere_value;
							ai.x = h;
							ai.y = i;
						}
						alpha = Mathf.Max(temp, alpha); //알파 값 찾기
						array[h, i] = 0;
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
		else
		{ //최소점수 <사람>
			int temp = 99999999;
			bool check_cut = false;
			for (int i = 0; i < 19; i++)
			{
				for (int h = 0; h < 19; h++)
				{
					if (array[h, i] == 0)
					{
						array[h, i] = 1;
						pre_x = (int)stone.x;
						pre_y = (int)stone.y;
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
	{ //점수 계산
		int team = 1;
		int temp = 0; //총점수		
		int x_score = (int)loca.x, y_score = (int)loca.y; //curloca
		Vector2 end1 = new Vector2(loca.x, loca.y), end2 = new Vector2(loca.x, loca.y); //계산하는 바둑의 양쪽 끝 좌표
		if (turn == false) { team = 1; }
		else { team = 2; }
		int cnt = 1, addvalue = 1;
		bool twist = true;
		while (true)
		{ //가로 => 이어져있는 같은 돌의 갯수세기
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
				if (array[x_score + addvalue, y_score] == team)
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
				if (array[x_score + addvalue, y_score] == team)
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
			{ //한쪽이 막혀있는데 판의 끝이 아님 <상대방이 막은 것>
				andscore += score1;
			}
			else
			{ //<0과 15인 경계선>
				andscore += score2;
			}
		}
		else if (sidecheck1 && sidecheck2)
		{ //둘쪽 열림
			andscore += score3;
		}
		else
		{ //양쪽이 막혀있는것 
			andscore = 0;
		}
		temp += andscore;
		///////////////////////////////////////////////////// /////////////////////////
		cnt = 1;
		addvalue = 1;
		twist = true;
		while (true)
		{ //세로  => 이어져있는 같은 돌의 갯수세기
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
				if (array[x_score, y_score + addvalue] == team)
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
				if (array[x_score, y_score + addvalue] == team)
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
				andscore += score1;
			}
			else
			{
				andscore += score2;
			}
		}
		else if (sidecheck1 && sidecheck2)
		{ //둘쪽 열림
			andscore += score3;
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
		{ //대각선1 (역슬래시 모양)  => 이어져있는 같은 돌의 갯수세기
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
				if (array[x_score + addvalue2, y_score + addvalue] == team)
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
				if (array[x_score + addvalue2, y_score + addvalue] == team)
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
				andscore += score1;
			}
			else
			{
				andscore += score2;
			}
		}
		else if (sidecheck1 && sidecheck2)
		{ //둘쪽 열림
			andscore += score3;
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
		{ //대각선2 (슬래시모양)  => 이어져있는 같은 돌의 갯수세기
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
				if (array[x_score + addvalue2, y_score + addvalue] == team)
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
				if (array[x_score + addvalue2, y_score + addvalue] == team)
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
		{ //한쪽만 열림
			if ((end1.x != 18 || end1.y != 0) || (end1.x != 0 || end1.y != 18))
			{
				andscore += score1;
			}
			else
			{
				andscore += score2;
			}
		}
		else if (sidecheck1 && sidecheck2)
		{ //둘쪽 열림
			andscore += score3;
		}
		else
		{
			andscore = 0;
		}
		temp += andscore;
		return temp;
	}

	public bool sideecheck(int x, int y, int direction)
	{ //길이 열려있는지 체크
		if (direction == 1)
		{ //가로
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
		{ //세로
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
		{ //대각선1
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
		{ //대각선2
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

	public void check()
	{ 
        Omok_play_data = new ArrayList();
		if (White_toggle.isOn)
		{
			back_gorund.SetActive(false);
			sel_col_Canvas.SetActive(false);
            AIColor_Black.SetActive(true);
            PlayerColor_White.SetActive(true);
            System.Threading.Thread.Sleep(500);
			okay_click = true;
			array[9, 9] = 2;
			AI = black_dol;
			PLAYER = white_dol;
			double location_x = 26.4 + 44.5 * 9 - 40 / 2;
			double location_y = 26.4 + 44.5 * (18 - 9) - 40 / 2;
			double return_x = location_x - 427;
			double return_y = location_y - 427;
			GameObject temp = Instantiate(AI);
			temp.transform.localScale = Vector2.one;
			temp.transform.SetParent(GameObject.Find("Canvas/OmokPan").transform, false);
			temp.transform.localPosition = new Vector2((float)return_x + 20f, (float)return_y + 20f);
			Omok_play_data.Add(new Vector2(9, 9));
		}
		else
		{
			System.Threading.Thread.Sleep(100);
			okay_click = true;
            AIColor_White.SetActive(true);
            PlayerColor_Black.SetActive(true);
            back_gorund.SetActive(false);
			sel_col_Canvas.SetActive(false);
			first_play = false;
			AI = white_dol;
			PLAYER = black_dol;
		}
	}

    public void exit()
	{
		SceneManager.LoadScene("Main");
	}
}