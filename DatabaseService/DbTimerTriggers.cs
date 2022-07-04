using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DatabaseService
{
    public enum TTriggerType : ushort
    {
        UNKNOWN = 0,
        CONNECT_DB = 1,
        DISCONNECT_DB = 2,
        READ = 3,
        WRITE = 4,
        UPDATE = 5,
        DELETE = 6,
        DELETE_ALL = 7,
        SEARCH = 8,
    }

    public class TimerTrigger
    {
        public TTriggerType TriggerType = TTriggerType.UNKNOWN;
        public DateTime DT = DateTime.Now;

        public TimerTrigger(TTriggerType triggerType, DateTime dt)
        {
            TriggerType = triggerType;
            DT = dt;
        }
    }


    public class TimerTriggers : List<TimerTrigger>
    {
        public void ClearAllTriggers()
        {
            try
            {
                Monitor.Enter(this);

                Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        public TimerTrigger FindTimerTrigger(TTriggerType triggerType)
        {
            TimerTrigger retTrigger = null;

            try
            {
                Monitor.Enter(this);

                int index = FindIndex(p => p.TriggerType == triggerType);

                if (index >= 0)
                {
                    retTrigger = this.ElementAt(index);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(this);
            }

            return retTrigger;
        }

        public bool DeleteTrigger(TTriggerType triggerType)
        {
            bool retVal = false;

            try
            {                

                Monitor.Enter(this);

                int index = FindIndex(p => p.TriggerType == triggerType);

                if (index >= 0)
                {
                    this.RemoveAt(index);
                    retVal = true;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(this);                
            }

            return retVal;


        }

        public TimerTrigger AddTimerTrigger(TTriggerType triggerType, DateTime dt)
        {
            TimerTrigger retTrigger = null;

            try {
                //if (FindTimerTrigger(triggerType) == null)
                {
                    retTrigger = new TimerTrigger(triggerType, dt.ToUniversalTime());

                    try
                    {
                        Monitor.Enter(this);

                        Add(retTrigger);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retTrigger;
        }

        public TimerTrigger GetPendingTriggerByTime(DateTime dt)
        {
            TimerTrigger retTrigger = null;

            try
            {
                Monitor.Enter(this);

                int index = FindIndex(p => (DateTime.Compare(p.DT, dt.ToUniversalTime())) <= 0);

                if (index >= 0)
                {
                    retTrigger = this.ElementAt(index);
                    this.RemoveAt(index);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(this);
            }

            return retTrigger;
        }
    }
}
