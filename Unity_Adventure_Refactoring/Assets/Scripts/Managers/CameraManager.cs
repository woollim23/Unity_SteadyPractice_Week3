using UnityEngine;

public class CameraManager : Singletone<CameraManager>
{
    private bool isOriginMain = true;
    public Camera originMainCamera; // 메인 카메라
    public Camera newMainCamera; // 3인칭 카메라

    public void SwitchMainCamera()
    {
        // 현재 메인 카메라를 비활성화
        if (Camera.main != null)
        {
            Camera.main.gameObject.SetActive(false);
        }

        if(isOriginMain)
        {
            // 3인칭 카메라를 활성화하고 메인 카메라로 설정
            newMainCamera.gameObject.SetActive(true);
            isOriginMain = false;
        }
        else
        {
            originMainCamera.gameObject.SetActive(true);
            isOriginMain = true;
        }
    }
}
