using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Responsible for Creating Enemies and placing them into the Enemy Container
/// </summary>
public class EnemyCreator : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    Transform EnemyContainer;

    [SerializeField]
    [Tooltip ("Offset from 0 to start drawing Enemies.")]
    float YOffset = 1;

    [SerializeField]
    [Tooltip("Enemy Row Count")]
    int Rows = 5;

    [SerializeField]
    [Tooltip("Enemy Column Count")]
    int Columns = 11;

    [SerializeField]
    float XSpacing = 1.0f;

    [SerializeField]
    float YSpacing = 1.0f;

#pragma warning restore 0649

    public GameObject[] EnemyPrefabs; // An array of enemies to create; must be set up in the editor

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<EnemyController> CreateEnemies()
    {
        for (int cnt = 0; cnt < EnemyContainer.childCount; cnt++) Destroy(EnemyContainer.GetChild(cnt).gameObject);

        List<EnemyController> enemyControllers = new List<EnemyController>();

        int enemyIndex = 0;

        // Number of enemies per row
        int enemyColumnCount = Columns;
        // Find midpoint
        float midCount = (enemyColumnCount / 2.0f) - 0.5f;

        for (int ycnt = 0; ycnt < Rows; ycnt++)
        {
            if (ycnt == 2) enemyIndex = 1;
            if (ycnt == 4) enemyIndex = 2;
            for (int xcnt = 0; xcnt < enemyColumnCount; xcnt++)
            {

                Vector3 pos = new Vector3(XSpacing * (((float)xcnt) - midCount), YSpacing * ((float)ycnt + YOffset), 0);

                GameObject newEnemy = Instantiate(EnemyPrefabs[enemyIndex]) as GameObject;
                newEnemy.transform.SetParent(EnemyContainer);
                newEnemy.transform.localPosition = pos;

                EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
                enemyControllers.Add(enemyController);

            }
        }

        return enemyControllers;
    }
}
