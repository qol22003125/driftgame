using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System.IO;
using UnityEngine.InputSystem.Controls;
[System.Serializable]
public class StylePointsData
{
    public float totalPoints;
}
public class StylePoints : MonoBehaviour
{
    private float Points;
    public bool IsCar;
    private float TotalPoints;
    private float PointsCooldown;
    private float MultiplierEligibility;
    private float Mult = 1;
    public TMP_Text DriftPointsUI;
    public TMP_Text MultiplierUI;
    string savePath;
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "style_points.json");
        LoadPoints();

        DriftPointsUI.text = "Drift Points: " + Mathf.Floor(TotalPoints).ToString();

        Debug.Log(TotalPoints);
    }
    void GetAngle()
    {
        Vector3 forward = transform.position;
        Vector3 right = transform.position + (transform.TransformDirection(Vector3.right) * GetComponent<Rigidbody>().angularVelocity.magnitude);
        float Angle = Vector3.SignedAngle(right, forward, Vector3.up);
        float gain = Mathf.Abs(Angle * Time.deltaTime) * 10 * Mult;
        TotalPoints += gain;
        if (Mathf.Abs(Angle * Time.deltaTime) * 50 > 0.1f)
        {
            Points += gain;
            MultiplierEligibility += (10 / (Mult * 0.35f)) * Time.deltaTime;
            if (MultiplierEligibility > 100)
            {
                MultiplierEligibility = 0;
                Mult += 1;
            }
            PointsCooldown = Time.time + 0.5f;
        }
        if (PointsCooldown < Time.time)
        {
            Points = math.max(Points - 1, 0);
            Mult = 1;
        }
        DriftPointsUI.text = Mathf.Floor(Points).ToString();
        MultiplierUI.text = "x" + Mathf.Floor(Mult).ToString();
        SavePoints();
    }
    void SavePoints()
    {
        StylePointsData data = new StylePointsData();
        data.totalPoints = TotalPoints;
        File.WriteAllText(savePath, JsonUtility.ToJson(data));
    }
    void LoadPoints()
    {
        if (!File.Exists(savePath)) return;
        StylePointsData data = JsonUtility.FromJson<StylePointsData>(File.ReadAllText(savePath));
        TotalPoints = data.totalPoints;
    }
    void Update()
    {
        if (IsCar)
        {
            GetAngle();
        }
    }
    void OnApplicationQuit()
    {
        SavePoints();
    }
}