namespace LinqToRest.LinqToRest
{
    internal enum ExpressionType
    {
        /// <summary>
        /// 1000 is an arbitrary value that is hight enough to not cause overlap with Linq.ExpressionType
        /// </summary>
        /// 
        /// <remarks>
        /// See https://blogs.msdn.microsoft.com/mattwar/2007/08/03/linq-building-an-iqueryable-provider-part-v/
        /// > I went ahead and defined my own DbExpressionType enum that ‘extends’ the base ExpressionType enum
        /// > by picking a sufficiently large starting value to not collide with the other definitions.
        /// > If there was such a way as to derive from an enum I would have done that, but this will work
        /// > as long as I am diligent.
        /// </remarks>
        Resource = 1_000,
        Projection,
        Field,
    }
}