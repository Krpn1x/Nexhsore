using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace DynamicTicketingAPI.Models
{
    public class CommandParameter
    {
        public CommandParameter()
        {

        }
        //Contains all managed resources that class uses.
        private Component objComponent = new Component();

        #region IDisposable Members
        /// <summary>
        /// This function is used for disposing all the managed 
        /// resource used by this class instance
        /// </summary>
        public void Dispose()
        {
            //Disposing  managed resources
            Dispose(true);
            //Call this to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (objComponent != null)
                {
                    this.objComponent.Dispose();
                    this.objComponent = null;
                }
            }


        }

        #endregion

        /// <summary>
        /// Property is used to set  and get the PrmType of the parameter
        /// </summary>
        public PrmType pDbType
        {
            get;
            set;
        }
        /// <summary>
        /// Property is used to set  and get the Name of the parameter
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Property is used to set  and get the Value of the parameter
        /// </summary>
        public object Value
        {
            get;
            set;
        }
        /// <summary>
        /// Property is used to set  and get the Direction of the parameter
        /// </summary>
        public PrmDirection Direction
        {
            get;
            set;
        }
        /// <summary>
        /// Property is used to set  and get the Size of the parameter
        /// </summary>
        public int Size
        {
            get;
            set;
        }
        /// <summary>
        /// Property is used to set and get the name of the source column
        /// </summary>
        public string SourceColumName
        {
            get;
            set;
        }
        /// <summary>
        /// Property is used to set  and get the Data row version
        /// </summary>
        public PrmDataRowVersion? RowVersion
        {

            get;
            set;
        }
        /// <summary>
        /// This is the cunstructor of the class used to set the values in the proerties
        /// </summary>
        public CommandParameter(PrmType prmDbType, string strName, object strValue, PrmDirection prmDirection, int iSize)
        {
            try
            {
                //set value sin the properties.
                pDbType = prmDbType;
                Name = strName;
                Value = strValue;
                Direction = prmDirection;
                Size = iSize;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// cunstructor use for UpdateDataSet() function [Reflect all the Dataset rows  into the database table]
        /// This is the  of the class used to set the values in the proerties,
        /// when we are creating the parameter for UpdateDataSet() function we have to use
        /// this cunstuctor for creating the parameter.
        /// Instead of pasing parameter value and size, we have to pass source column name and Datarow version. 
        /// </summary>
        public CommandParameter(PrmType prmDbType, string strName, string strSourceColumn, PrmDataRowVersion PrmDataRowVersion)
        {
            try
            {
                //set value in the properties.
                pDbType = prmDbType;
                Name = strName;
                Direction = PrmDirection.Input;
                SourceColumName = strSourceColumn;
                RowVersion = PrmDataRowVersion;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// CreateInputParameter method of the class used to initilize the value for input type parameter
        /// </summary>
        /// <param name="prmDbType">PrmType of parameter</param>
        /// <param name="strName">Name of parameter</param>
        /// <param name="strValue">Value of parameter</param>
        /// <ReturnType> void</ReturnType>
        public void CreateInputParameter(PrmType prmDbType, string strName, object strValue)
        {
            try
            {
                this.CreateParameter(prmDbType, strName, strValue, PrmDirection.Input, 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// CreateOutputParameter method of the class used to initilize the value for Output type parameter
        /// </summary>
        /// <param name="prmDbType">PrmType of parameter</param>
        /// <param name="strName">Name of parameter</param>
        /// <param name="iSize">Size of parameter</param>
        /// <ReturnType> void</ReturnType>
        public void CreateOutputParameter(PrmType prmDbType, string strName, int iSize)
        {
            try
            {
                this.CreateParameter(prmDbType, strName, null, PrmDirection.Output, iSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        /// <summary>
        /// CreateParameter private method of the class used to initilize the value parameter
        /// </summary>
        /// <param name="prmDbType">PrmType of parameter</param>
        /// <param name="strName">Name of parameter</param>
        /// <param name="strValue">Value of parameter</param>
        ///  <param name="prmDirection">direction of parameter</param>
        /// <param name="iSize">Size of parameter</param>
        /// <ReturnType>void</ReturnType>
        private void CreateParameter(PrmType prmDbType, string strName, object strValue, PrmDirection prmDirection, int iSize)
        {

            try
            {
                //set value in the properties.
                this.pDbType = prmDbType;
                Name = strName;
                Value = strValue;
                Direction = prmDirection;
                Size = iSize;
                return;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}