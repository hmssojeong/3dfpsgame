using UnityEngine;

public class Monster : MonoBehaviour
{
    // 목표: 처음에는 가만히 있지만 플레이어가 다가가면 쫒아오는 좀비 몬스터를 만들고 싶다.
    //        ㄴ 쫒아 오다가 너무 멀어지면 제자리로 돌아간다.

    // 몬스터 인공지능(AI) : 사람처럼 행동하는 똑똑한 시스템/알고리즘
    // - 규칙 기반 인공지능 : 정해진 규칙에 따라 조건문/반복문 등을 이용해서 코딩하는 것
    //                        -> ex) FSM(유한 상태머신), BT(행동 트리)
    // - 학습 기반 인공지능: 머신러닝(딥러닝, 강화학습 ..)

    enum monsterstat
    {
        idle,
        move,
        die
    }

}
