using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class PlayerAgent : Agent
{

    public GameObject indiv;

    public GameObject[] aal;
    public bool[] alive_aal;
    [SerializeField]public bool team;
    [SerializeField]GameObject judge;
    public GameObject enemy;
    [SerializeField]public GameObject field;
    public int target;
    public Vector3 direction;
    public float strength;
    private float m_InvertMult;

    public bool myTurn;
    int death;
    int enemydeath;


    public override void InitializeAgent()
    {
        death = 0;
        enemydeath =0;
        aal = new GameObject[8];
        alive_aal = new bool[8];
        m_InvertMult = team ? -1f : 1f;
        // Game REST, 각 알들을 Initialize 해주자
        enemy = team ? judge.GetComponent<judge>().p2:judge.GetComponent<judge>().p1;
        SetResetParameters();
    }

    public override void CollectObservations()
    {
        //aal 들의 상태 파악하기
        int cnt = 0;
        for(int i = 0; i < aal.Length; i++){
            if(aal[i]== null){
                alive_aal[i] = false;// 아군의 죽음 카운트
                cnt++;
            }
        }
        if(cnt-death>0){
            AddReward(-(cnt-death)/20f);
            Debug.Log("Death reward " +team +"--"+ (cnt-death));
            death = cnt;
        }

        // 여기서는 Game Parameter 들들 벡터로 보내줘야 한다!!!
        // 보낼 것: 
        for(int i = 0; i < aal.Length;i++){
            if(aal[i] != null){
                AddVectorObs(m_InvertMult*(aal[i].transform.position.x - transform.position.x));
                AddVectorObs(aal[i].transform.position.y - transform.position.y);
                AddVectorObs(alive_aal[i] ? 0:1);
            }else{
                AddVectorObs(-10f);
                AddVectorObs(-10f);
                AddVectorObs(alive_aal[i] ? 0:1);
            }

        }

        PlayerAgent es = enemy.GetComponent<PlayerAgent>();
        cnt = 0;
        for(int i = 0; i < aal.Length; i++){
            if(es.aal[i]== null){
                es.alive_aal[i] = false;// 아군의 죽음 카운트
                cnt++;
            }
        }
        if(cnt-enemydeath>0){
            AddReward((cnt-enemydeath)/20f);
            Debug.Log("Kill reward " +team +"--"+ (cnt-enemydeath));
            enemydeath = cnt;
        }
        for(int i = 0; i < es.aal.Length;i++){
            if(es.aal[i]!= null){
                AddVectorObs(es.m_InvertMult*(es.aal[i].transform.position.x - es.transform.position.x));
                AddVectorObs(es.aal[i].transform.position.y - es.transform.position.y);
                AddVectorObs(es.alive_aal[i] ? 0:1);
            }else{
                AddVectorObs(-10f);
                AddVectorObs(-10f);
                AddVectorObs(alive_aal[i] ? 0:1);
            }

        }
        //Vector 갯수는 8 * 6
        //아군 상태와 적 상태를 전부 보내자.
    
    }

    public override void AgentAction(float[] vectorAction)
    {
        for(int i = 0; i < aal.Length; i++){
            if(aal[i]== null){
                alive_aal[i] = false;// 아군의 죽음 카운트
            }
        }
        //할 일:
        // 1. <어떤 알?>  <어느 방향으로> <얼마나 세게>
        var dir = Mathf.Clamp(vectorAction[1],0f,1f)*2*Mathf.PI;

        var idx = Mathf.RoundToInt(Mathf.Clamp(vectorAction[0],0f,1f)*7);
        var dirV = new Vector2(m_InvertMult*Mathf.Cos(dir),m_InvertMult*Mathf.Sin(dir));
        var strength = Mathf.Clamp(vectorAction[2],0f,1f)*400f;

        if(alive_aal[idx]==true){
            //알을 때리자
            GameObject target = aal[idx];
            Rigidbody2D targetRB = aal[idx].GetComponent<Rigidbody2D>();
            targetRB.AddForce(dirV*strength);
            AddReward(0.05f);
        }else{
            AddReward(-0.03f);
        }
        if(strength < 120f){
            AddReward(-0.03f);
            //Debug.Log("Death reward " +team +"--"+ cnt-death);
        }else{
            AddReward(0.03f);
        }

        //Debug.Log("ACTION "+team+": "+idx +" strength="+strength+" angle: "+dir);
        //때린 다음에, 자신이 끝났음을 신고하자.
        if(team){
            judge.GetComponent<judge>().p1fin = true;
        }else{
            judge.GetComponent<judge>().p2fin = true;
        }
    }

    public override float[] Heuristic()
    {
        var action = new float[2];

        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        return action;
    }

    public override void AgentReset()
    {
        m_InvertMult = team ? -1f : 1f;
        SetResetParameters();
    }

    public void SetResetParameters()
    {
        
        transform.position = field.transform.position-new Vector3(4*m_InvertMult,0,0);
        for(int i = 0; i < 8; i++){
            alive_aal[i] = true;
            if(aal[i]!= null){
                Destroy(aal[i]);
            }
            aal[i] = (GameObject)Instantiate(indiv, transform.position + new Vector3(0,i-3.5f,0), transform.rotation);
            if(team)aal[i].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
        }
    }
    private void FixedUpdate() {
        if(myTurn){
            RequestDecision();
            myTurn = false;
            if(team){
                judge.GetComponent<judge>().p1fin = true;
            }else{
                judge.GetComponent<judge>().p2fin = true;
            }
        }
    }
    
}
