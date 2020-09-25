using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private StageData stageData;            // 적 생성을 위한 스테이지 크기 정보
    [SerializeField]
    private GameObject enemyPrefab;         // 복제해서 생성할 적 캐릭터 프리팹
    [SerializeField]
    private GameObject enemyHpSliderPrefab; // 적 체력을 나타내는 Slider UI
    [SerializeField]
    private Transform canvasTransform;      // UI를 표현하는 Canvas 오브젝트의 Transform
    [SerializeField]
    private BgmController bgmController;   // 배경음악 설정
    [SerializeField]
    private GameObject panelBossHp;         // 보스 체력 패널 오브젝트
    [SerializeField]
    private GameObject textBossWarning;     // 보스 등장 텍스트 오브젝트
    [SerializeField]
    private GameObject boss;                // 보스 오브젝트
    [SerializeField]
    private float spawnTime;               // 적 생성 주기
    [SerializeField]
    private int maxEnemyCount = 100;        // 적 생성 숫자


    private void Awake()
    {
        textBossWarning.SetActive(false);
        panelBossHp.SetActive(false);
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        int currentEnemyCount = 0;
        while (true)
        {
            // x 위치는 스테이지 크기 범위 내에서 임의의 값을 선택
            float positionX = Random.Range(stageData.LimitMin.x, stageData.LimitMax.x);
            // 적 캐릭터 생성
            GameObject enemyClone = Instantiate(enemyPrefab, new Vector3(positionX, stageData.LimitMax.y + 1.0f, 0.0f), Quaternion.identity);
            // 적 HpSlider 생성
            SpawnEnemyHpSlder(enemyClone);

            currentEnemyCount++;
            if (currentEnemyCount == maxEnemyCount)
            {
                StartCoroutine("SpawnBoss");
                break;
            }
            // Spawn 딜레이
            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void SpawnEnemyHpSlder(GameObject enemy)
    {
        // 적 슬라이더 UI 생성
        GameObject sliderClone = Instantiate(enemyHpSliderPrefab);

        // Slider UI 오브젝트를 Canva오브젝트의 자식으로 설정

        sliderClone.transform.SetParent(canvasTransform);

        // 계층설정으로 바뀐 크기를 다시 (1,1,1)로 설정
        sliderClone.transform.localPosition = Vector3.one;

        // Slider가 UI 쫓아다니도록 대상을 설정
        sliderClone.GetComponent<SliderPositionAutoSetter>().SetUp(enemy.transform);

        // Slider UI에 체력 정보를 표시
        sliderClone.GetComponent<EnemyHpViewer>().SetUp(enemy.GetComponent<EnemyHp>());
    }

    private IEnumerator SpawnBoss()
    {
        bgmController.ChangeBgm(BGMType.Boss);
        textBossWarning.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        textBossWarning.SetActive(false);

        boss.SetActive(true);
        panelBossHp.SetActive(true);
        boss.GetComponent<Boss>().ChangeState(BossState.MoveToApperPoint);
    }
}
