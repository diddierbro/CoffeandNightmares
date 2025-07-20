using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Chr_Shooting : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mousePosition;
    [SerializeField]
    private GameObject bulletPrefab;

    public Transform bulletSpawnPoint;
    public bool canShoot;
    private float timer;
    public float timeBetweenShots;

    public bool isStressedMedium = false;
    public bool isStressedHigh = false;

    public float mediumrecoilAngle = 3f;
    public float highrecoilAngle = 8f;
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePosition - transform.position;

        float angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (!canShoot)
        {
            timer += Time.deltaTime;
            if (timer >= timeBetweenShots)
            {
                canShoot = true;
                timer = 0f;
            }
        }

        if (Input.GetMouseButton(0) && canShoot)
        {
            canShoot = false;
            float recoilAngle = 0f;
            if (isStressedHigh)
            {
                recoilAngle = Random.Range(-highrecoilAngle, highrecoilAngle);
            }
            else if (isStressedMedium)
            {
                recoilAngle = Random.Range(-mediumrecoilAngle, mediumrecoilAngle);
            }
            Quaternion recoilRotation = Quaternion.Euler(0, 0, angle + recoilAngle);
            GameObject bulletclone = Instantiate(bulletPrefab, bulletSpawnPoint.position, recoilRotation);
            Destroy(bulletclone, 2);
        }
    }
}
