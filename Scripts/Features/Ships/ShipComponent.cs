using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// ����� �������, ������ ��������� �� ��
    /// </summary>
    struct ShipComponent
    {
        public int Encounter;
        public int Wave;
        public List<int> EnemyUnitsEntitys;
        public ShipArrivalMB ShipArrivalMB;
    }
}