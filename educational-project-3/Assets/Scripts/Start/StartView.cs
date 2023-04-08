using Descriptions.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    public class StartView : MonoBehaviour
    {
        public DescriptionsCollectionSo DescriptionsCollection;

        public GameObject LoaderCanvas;
        public Image ProgressBar;
        public TextMeshProUGUI LoadingTxt;
        public TMP_InputField KeyInputField;
        public Button NewGameBtn;
    }
}