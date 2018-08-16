using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float Factoer=1;

    public float MaxDistance = 4;
    public GameObject Stage;

    //获取相机位置
    public Transform Camera;

    private Rigidbody _rigidbody;
    private float _stateTime;

    public GameObject Particle;

    public Transform Head;
    public Transform Body;

    //当前盒子物体
    private GameObject _currentStage;
    private Collider _lastCollisionCollider;

    //相机的相对位置
    public Vector3 _camerRelativePosition;

    public Text ScoreText;
    private int _score;

    Vector3 _direction = new Vector3(1, 0, 0);

	// Use this for initialization
	void Start () {
        _rigidbody = GetComponent<Rigidbody>();
        Particle = GameObject.Find("yellow");
        Particle.SetActive(false);
        //修改物理组件的重心到body的底部
        _rigidbody.centerOfMass = Vector3.zero;

        _currentStage = Stage;
        _lastCollisionCollider = _currentStage.GetComponent<Collider>();
        SpawnStage();

        _camerRelativePosition = Camera.position - transform.position;
        
    }
	
	// Update is called once per frame
	void Update () {
       
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _stateTime = Time.time;
            Particle.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            var elapse = Time.time - _stateTime;
            OnJump(elapse);
            Particle.SetActive(false);

            Body.transform.DOScale(0.1f,1);
            Head.transform.DOLocalMoveY(0.29f,0.5f);

            _currentStage.transform.DOLocalMoveY(0.25f,0.2f);
            _currentStage.transform.DOScale(new Vector3(1,0.5f,1),0.2f);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Body.transform.localScale += new Vector3(1, -1, 1) * 0.05f * Time.deltaTime;
            Head.transform.localPosition += new Vector3(0,-1,0)*0.1f*Time.deltaTime;

            //盒子缩放沿着轴心缩放
            _currentStage.transform.localScale += new Vector3(0, -1, 0)*0.15f*Time.deltaTime;
            _currentStage.transform.localPosition += new Vector3(0, -1, 0) * 0.15f * Time.deltaTime;
        }

	}

    void OnJump(float elapse)
    {
        _rigidbody.AddForce((new Vector3(0,1,0)+_direction)*elapse* Factoer,ForceMode.Impulse);
    }

    void SpawnStage()
    {
        var stage = Instantiate(Stage);
        stage.transform.position = _currentStage.transform.position + _direction * Random.Range(1.1f,MaxDistance) ;


        var randomScale = Random.Range(0.5f,1);
        Stage.transform.localScale = new Vector3(randomScale, 0.5f, randomScale);

        stage.GetComponent<Renderer>().material.color = new Color(Random.Range(0.01f, 1), Random.Range(0.01f, 1), Random.Range(0.01f, 1));

    }

    //如果有别的物体和本物体发生变化，会触发这函数
    void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.name.Contains("Stage") && collision.collider!=_lastCollisionCollider)
        {
            _lastCollisionCollider = collision.collider;
            _currentStage = collision.gameObject;
            RandomDirection();
            SpawnStage();
            MoveCamera();

            _score++;
            ScoreText.text = _score.ToString();
        }

       if(collision.gameObject.name=="Ground")
        {
            //本局游戏结束，重新开始
            SceneManager.LoadScene("Gary");
        }
    }

    void RandomDirection()
    {
        var seed = Random.Range(0,2);
        if(seed == 0)
        {
            _direction = new Vector3(1, 0, 0);
        }
        else
        {
            _direction = new Vector3(0,0,1);
        }
        
    }

    void MoveCamera()
    {
         Camera.DOMove(transform.position + _camerRelativePosition,1);
    }
}
