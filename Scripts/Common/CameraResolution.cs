using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private void Awake()
    {
        //���� �� �ػ� ����
        Screen.SetResolution(2400, 1080, FullScreenMode.Windowed);
        Camera cam = GetComponent<Camera>();
        Rect viewportRect = cam.rect;
        float screenAspectRatio = (float)Screen.width / Screen.height;
        float targetAspectRatio = 2400f / 1080f;
        if (screenAspectRatio < targetAspectRatio)
        {
            viewportRect.height = screenAspectRatio / targetAspectRatio;
            viewportRect.y = (1f - viewportRect.height) / 2f;
        }
        else
        {
            viewportRect.width = targetAspectRatio / screenAspectRatio;
            viewportRect.x = (1f - viewportRect.width) / 2f;
        }
        cam.rect = viewportRect;
    }
}
