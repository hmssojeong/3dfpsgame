using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ClickMove : MonoBehaviour
{
    NavMeshAgent agent;
    FPSTPSCameraController cameraController;

    public Transform Spot;

    LineRenderer lr;
    Coroutine draw;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cameraController = Camera.main.GetComponent<FPSTPSCameraController>();

        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material.color = Color.green;
        lr.enabled = false;
    }

    private void Update()
    {
        // 탑뷰일 때만 클릭 이동 가능
        if (cameraController == null || cameraController.CurrentMode != FPSTPSCameraController.CameraMode.TopView)
        {
            if (Spot != null && Spot.gameObject.activeSelf)
            {
                Spot.gameObject.SetActive(false);
            }
            if (lr != null && lr.enabled)
            {
                lr.enabled = false;
            }
            if (draw != null)
            {
                StopCoroutine(draw);
                draw = null;
            }
            return;
        }

        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);

                Spot.gameObject.SetActive(true);
                Spot.position = hit.point;

                if (draw != null)
                {
                    StopCoroutine(draw);
                }

                draw = StartCoroutine(DrawPath_Coroutine());
            }
        }
        else if (agent.remainingDistance < 0.1f)
        {
            Spot.gameObject.SetActive(false);

            lr.enabled = false;
            if (draw != null)
            {
                StopCoroutine(draw);
            }
        }
    }

    IEnumerator DrawPath_Coroutine()
    {
        lr.enabled = true;
        yield return null;
        while (true)
        {
            int cnt = agent.path.corners.Length;
            lr.positionCount = cnt;
            for (int i = 0; i < cnt; i++)
            {
                lr.SetPosition(i, agent.path.corners[i]);
            }
            yield return null;
        }
    }
}
