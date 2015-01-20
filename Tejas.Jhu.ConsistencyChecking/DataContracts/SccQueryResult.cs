using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tejas.Jhu.GraphUtilities.GraphBusinessObjects;

namespace Tejas.Jhu.ConsistencyChecking.DataContracts
{
    public class SccQueryResult
    {
        public List<VertexProperties> SccComponentList { get; set; }
        public VertexProperties SccVertexOne { get; set; }
        public VertexProperties SccVertexTwo { get; set; }

        public SccQueryResult(List<VertexProperties> sccComponentList, VertexProperties sccVertexOne, VertexProperties sccVertexTwo)
        {
            SccComponentList = sccComponentList;
            SccVertexOne = sccVertexOne;
            SccVertexTwo = sccVertexTwo;
        }
    }
}
