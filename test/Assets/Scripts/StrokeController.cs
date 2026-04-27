using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrokeController : MonoBehaviour
{
    [SerializeField] Material lineMaterial;
    [SerializeField] Color lineColor;
    [Range(0.1f, 0.5f)]
    [SerializeField] float lineWidth;

    [SerializeField] PhysicsMaterial2D bounceMaterial;
    [SerializeField] float lifeTime = 3f;

    [Header("Line Limit")]
    [SerializeField] float maxLength = 5f;
    float currentLength = 0f;

    [Header("UI")]
    [SerializeField] Image gauge;

    class TimedPoint
    {
        public Vector2 position;
        public float time;

        public TimedPoint(Vector2 pos, float t)
        {
            position = pos;
            time = t;
        }
    }

    class LineData
    {
        public GameObject obj;
        public LineRenderer renderer;
        public EdgeCollider2D collider;
        public List<TimedPoint> points = new List<TimedPoint>();
    }

    List<LineData> lines = new List<LineData>();
    LineData currentLine;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _createLine();
            currentLength = 0f; // リセット
        }

        if (Input.GetMouseButton(0))
        {
            _addPoint();
        }

        _updateAllLines();
        _updateGauge();
    }

    private void _createLine()
    {
        GameObject obj = new GameObject("Line");
        obj.tag = "Ground";

        LineRenderer lr = obj.AddComponent<LineRenderer>();
        EdgeCollider2D col = obj.AddComponent<EdgeCollider2D>();
        col.sharedMaterial = bounceMaterial;

        obj.transform.SetParent(transform);

        lr.material = lineMaterial;
        lr.material.color = lineColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 0;

        currentLine = new LineData
        {
            obj = obj,
            renderer = lr,
            collider = col
        };

        lines.Add(currentLine);
    }

    private void _addPoint()
    {
        if (currentLine == null) return;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // 最初の点
        if (currentLine.points.Count == 0)
        {
            currentLine.points.Add(new TimedPoint(worldPos, Time.time));
            return;
        }

        Vector2 lastPos = currentLine.points[currentLine.points.Count - 1].position;
        float dist = Vector2.Distance(lastPos, worldPos);

        // 制限チェック
        if (currentLength + dist > maxLength)
        {
            return;
        }

        currentLength += dist;

        currentLine.points.Add(new TimedPoint(worldPos, Time.time));
    }

    private void _updateGauge()
    {
        if (gauge != null)
        {
            gauge.fillAmount = currentLength / maxLength;
        }
    }

    private void _updateAllLines()
    {
        float now = Time.time;
        bool isDrawing = Input.GetMouseButton(0);

        for (int l = lines.Count - 1; l >= 0; l--)
        {
            var line = lines[l];

            line.points.RemoveAll(p => now - p.time > lifeTime);

            if (line.points.Count < 2 && !isDrawing)
            {
                Destroy(line.obj);
                lines.RemoveAt(l);
                continue;
            }

            line.renderer.positionCount = line.points.Count;

            for (int i = 0; i < line.points.Count; i++)
            {
                line.renderer.SetPosition(i, line.points[i].position);
            }

            if (line.points.Count >= 2)
            {
                List<Vector2> pts = new List<Vector2>();
                foreach (var p in line.points)
                {
                    pts.Add(p.position);
                }

                line.collider.SetPoints(pts);
            }
        }
    }
}