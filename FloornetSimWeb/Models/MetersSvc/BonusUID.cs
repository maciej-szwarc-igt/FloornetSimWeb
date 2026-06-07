using System;
using System.Collections.Generic;
using System.Text;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.MetersSvc
{
    public class BonusUID
    {
        /// <summary>
        /// mLevelId = Bonus Id + PoolId
        /// </summary>
        private ushort mLevelId;

        /// <summary>
        /// Id of the Bonus Controller
        /// </summary>
        public byte BonusId { get { return ((byte)((mLevelId & 0xFF00) >> 8)); } }

        /// <summary>
        /// Id of the bonus pool (0-15)
        /// </summary>
        public byte PoolId { get { return ((byte)(mLevelId & 0x00FF)); } }

        /// <summary>
        /// BSId or LevelId (combination of Bonus Id and Pool Id)
        /// </summary>
        public ushort LevelId { get { return mLevelId; } }


        public BonusUID(long levelId)
        {
            mLevelId = (ushort)levelId;
        }

        public BonusUID(byte bonusId, byte poolId)
        {
            SetBonus(bonusId);
            SetPool(poolId);
        }

        private void SetBonus(byte bonusid)
        {
            mLevelId = (ushort)((mLevelId & 0x00FF) | ((ushort)bonusid << 8));
        }

        private void SetPool(byte pool)
        {
            mLevelId = (ushort)((mLevelId & 0xFF00) | pool);
        }

        public override int GetHashCode()
        {
            return mLevelId.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", BonusId, PoolId);
        }


    }
}
