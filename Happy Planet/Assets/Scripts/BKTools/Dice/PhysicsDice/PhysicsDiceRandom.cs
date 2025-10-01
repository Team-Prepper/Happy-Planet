using UnityEngine;
using System;

namespace BKTools.Gaming.Dice
{

    public class PhysicsDiceRandom
    {
        private System.Random _rand = new System.Random(-1);

        public void SetSeed(int seed)
        {
            _rand = new System.Random(seed);

        }

        public Vector3 InsideUnitSphere
        {

            get
            {

                // 1. 랜덤 방향 생성 (θ: [0, 2π], ϕ: [0, π])
                double theta = 2 * Math.PI * _rand.NextDouble();
                double phi = Math.Acos(1 - 2 * _rand.NextDouble()); // 변환된 φ 범위 조정

                // 2. 단위 벡터 변환
                float x = (float)(Math.Cos(theta) * Math.Sin(phi));
                float y = (float)(Math.Sin(theta) * Math.Sin(phi));
                float z = (float)(Math.Cos(phi));

                // 3. 반지름 r^(1/3) 적용하여 균일 분포 유지
                float r = (float)Math.Pow(_rand.NextDouble(), 1.0 / 3.0);

                // 4. 최종 벡터 반환
                return new Vector3(x, y, z) * r;
            }

        }
    }
}