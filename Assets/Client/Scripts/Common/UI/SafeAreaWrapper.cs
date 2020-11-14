using UnityEngine;

namespace Client.Common
{
    public class SafeAreaWrapper : MonoBehaviour
    {
        void Awake()
        {
            var safeArea = Screen.safeArea;
            if (safeArea.size.x < Screen.width || safeArea.size.y < Screen.height)
            {
                if (TryGetComponent(out RectTransform rectTransform))
                {
                    rectTransform.anchorMin = new Vector2(safeArea.position.x / Screen.width,
                        safeArea.position.y / Screen.height);
                    rectTransform.anchorMax = new Vector2((safeArea.position.x + safeArea.size.x) / Screen.width,
                        (safeArea.position.y + safeArea.size.y) / Screen.height);
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;
                }
            }
        }
    }
}