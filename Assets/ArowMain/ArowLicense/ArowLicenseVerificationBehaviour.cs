using System.Collections;
using ArowMain.Runtime.License;
using UnityEngine;

namespace ArowMain
{
public class ArowLicenseVerificationBehaviour : MonoBehaviour
{
    [SerializeField] private LicenseData licenseData = null;
    [SerializeField] private bool isPlayOnVerification = true;

    public LicenseVerificationManager LicenseInstance => LicenseVerificationManager.GetUniqueInstance();

    void Start()
    {
        if (licenseData == null)
        {
            Debug.LogError("LicenseData is not set.");
            return;
        }

        if (isPlayOnVerification)
        {
            StartVerificationRequest();
        }
    }

    /// <summary>
    /// ライセンス検証の通信を開始する。
    /// </summary>
    public void StartVerificationRequest()
    {
        if (!LicenseInstance.IsValid)
        {
            var request = CreateVerificationRequest();
            StartCoroutine(request);
        }
    }

    private IEnumerator CreateVerificationRequest()
    {
        return LicenseInstance.CreateVerificationRequest(licenseData);
    }
}
}