using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class judge : MonoBehaviour
{
    [SerializeField] public GameObject p1;
    [SerializeField] public GameObject p2;
    PlayerAgent p1script;
    PlayerAgent p2script;
    GameObject[] p1aals;
    GameObject[] p2aals;

    public bool p1fin;
    public bool p2fin;
    bool p1Turn;
    // Start is called before the first frame update
    //심판의 역할:: 
    /*
    1. 각 Agent 들의 알들의 상태를 추적 (활성/비활성)
    2. Agent 들의 게임 순서 정하기
    3. Agent들에게 점수 주기 (ML 을 위한)
    4. 승패 인식. Reset 함수 전달
    */
    //p1 부터 시작을 궁룰로 하자.
    void Start()
    {
        p1script = p1.GetComponent<PlayerAgent>();
        p2script = p2.GetComponent<PlayerAgent>();
        p1fin = false;
        p2fin = false;
        p1script.RequestDecision();


        
        //이제, p1이 먼저 하게 하자!
    }

    // Update is called once per frame
    void Update()
    {

        if(p1fin){
            //Debug.Log("got p1fin");
            p1aals = p1script.aal;
            p2aals = p2script.aal;
            int stopcount = 0;
            int P1wincount = 0;
            int P2wincount = 0;
            for(int i = 0; i < p1aals.Length;i++){
                if(p1aals[i] == null){
                    stopcount++;
                    P2wincount++;
                }else if(p1aals[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero){
                    stopcount++;
                }
                if(p2aals[i] == null){
                    stopcount++;
                    P1wincount++;
                }else if(p2aals[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero){
                    stopcount++;
                }
            }
            if(stopcount == 2*p1aals.Length){
                //Debug.Log("p1 stop");
                p1fin = false;
                p1Turn = false;// 턴 끝!
                p2script.myTurn = true;
                p2script.RequestDecision();
            }
            if(P1wincount == p2aals.Length){
                p1.GetComponent<PlayerAgent>().AddReward(1.0f);
                Debug.Log("P1 Win reward");

                p2.GetComponent<PlayerAgent>().Done();
                p1.GetComponent<PlayerAgent>().Done();
            }else if(P2wincount == p1aals.Length){
                p2.GetComponent<PlayerAgent>().AddReward(1.0f);
                Debug.Log("P2 Win reward");

                p2.GetComponent<PlayerAgent>().Done();
                p1.GetComponent<PlayerAgent>().Done();
            }

        }else if(p2fin){
            //Debug.Log("got p2fin");
            p1aals = p1script.aal;
            p2aals = p2script.aal;
            int stopcount = 0;
            int P1wincount = 0;
            int P2wincount = 0;
            for(int i = 0; i < p1aals.Length;i++){
                if(p1aals[i] == null){
                    stopcount++;
                    P2wincount++;
                }else if(p1aals[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero){
                    stopcount++;
                }
                if(p2aals[i] == null){
                    stopcount++;
                    P1wincount++;
                }else if(p2aals[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero){
                    stopcount++;
                }
            }
            if(stopcount == 2*p2aals.Length){
                //Debug.Log("p2 stop");
                p2fin = false;
                p1Turn = true;// 턴 끝!
                p1script.myTurn = true;
                p1script.RequestDecision();
            }
            if(P2wincount == p1aals.Length){
                p1.GetComponent<PlayerAgent>().AddReward(1.0f);

                p2.GetComponent<PlayerAgent>().Done();
                p1.GetComponent<PlayerAgent>().Done();
            }else if(P1wincount == p2aals.Length){
                p2.GetComponent<PlayerAgent>().AddReward(1.0f);

                p2.GetComponent<PlayerAgent>().Done();
                p1.GetComponent<PlayerAgent>().Done();
            }
        }
    }
}
