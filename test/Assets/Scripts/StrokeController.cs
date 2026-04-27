using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StrokeController : MonoBehaviour
{
    [SerializeField] Material lineMaterial;
    [SerializeField] Color lineColor;
    [Range(0.1f, 0.5f)]
    [SerializeField] float lineWidth;

    [SerializeField]PhysicsMaterial2D bounceMaterial;
    GameObject lineObj;         //線のオブジェクトとなる変数
    LineRenderer lineRenderer;  //線の描画に必要なコンポーネント
    List<Vector2> linePoints;   //線の座標

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Listの初期化
        linePoints = new List<Vector2>();

    }

    // Update is called once per frame
    void Update()
    {
        //左クリックされたとき
        if (Input.GetMouseButtonDown(0))
        {
            //線となるオブジェクト作成
            _addLineObject();
        }
        //左クリックされ続けているとき
        if (Input.GetMouseButton(0))
        {
            //lineRendererの更新処理
            _addPositionDataToLineRenderer();
        }
    }

    //ゲームオブジェクト作成関数
    private void _addLineObject()
    {
        lineObj = new GameObject();             //ゲームオブジェクト作成
        lineObj.name = "Line";                  //オブジェクトの名前を"Line"にする
        lineObj.tag = "Ground";
        lineObj.AddComponent<LineRenderer>();   //LineRendererを追加
        lineObj.AddComponent<EdgeCollider2D>(); //EdgeCollider2Dを追加

        EdgeCollider2D col = lineObj.GetComponent<EdgeCollider2D>();
        col.sharedMaterial = bounceMaterial;

        lineObj.transform.SetParent(transform); //オブジェクトを自身(スクリプトをアタッチしているオブジェクト)の子に設定

        linePoints = new List<Vector2>();

        //lineRenderer初期化処理
        _initRenderer();
    }

    //LineRenderer初期化関数
    private void _initRenderer()
    {
        lineRenderer = lineObj.GetComponent<LineRenderer>(); //LineRendererを取得
        lineRenderer.positionCount = 0;                      //ポジションカウントリセット
        lineRenderer.material = lineMaterial;                //マテリアルを設定
        lineRenderer.material.color = lineColor;             //マテリアルの色を設定
        lineRenderer.startWidth = lineWidth;                 //始点の太さを設定
        lineRenderer.endWidth = lineWidth;                   //終点の太さを設定
    }
    
    //lineRenderer更新処理
    private void _addPositionDataToLineRenderer()
    {
          /*座標に関する処理*/
        //マウス座標取得
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.0f);
        //ワールド座標へ変換
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        
        /*LineRendererに関する処理*/
        //LineRendererのポイントを増やす
        lineRenderer.positionCount += 1;
        //LineRendererのポジションを設定
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, worldPos);
        
        /*EdgeCollider2Dに関する処理*/
        //ワールド座標をリストに追加
        linePoints.Add(worldPos);
        //EdgeCollider2Dのポイントを設定
        lineObj.GetComponent<EdgeCollider2D>().SetPoints(linePoints);
    }
}