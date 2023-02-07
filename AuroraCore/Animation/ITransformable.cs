///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Animation
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransformable
    {
        Transform Transform { get; set; }
        ITransformable TransformableParent { get; set; }      
    }
}
