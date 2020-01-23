using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundActivity : MonoBehaviour {
    [SerializeField] GameObject[] backgroundCars;
    float cameraWidth;
    float cameraHight;
    public int totalBackroundObjects = 0;

    void Start () {
        cameraHight = 2 * Camera.main.orthographicSize;
        cameraWidth = cameraHight * Camera.main.aspect;
    }

    public void SpawnBackgroundActivity()
    {
        if (totalBackroundObjects < 5)
        {
            int randIndex = Random.Range(0, backgroundCars.Length);
            float randY = Random.Range(1, cameraHight - 3);
            int randSign = Random.Range(-1, 1) == 0 ? 1 : -1;
            Vector2 pos = new Vector2(Camera.main.transform.position.x + randSign * 3 / 2 * cameraWidth, randY);
            GameObject car = Instantiate(backgroundCars[randIndex], pos, transform.rotation);
            car.GetComponent<Car>().SetBackActivity(this, cameraWidth);
            car.transform.localScale = new Vector3(car.transform.localScale.x * randSign, car.transform.localScale.y, 0);
            totalBackroundObjects++;
        }
    }

    IEnumerator DestroyCar(float carSpeed)
    {
        float time = cameraWidth / carSpeed;
        yield return new WaitForSeconds(time);
    }
}
