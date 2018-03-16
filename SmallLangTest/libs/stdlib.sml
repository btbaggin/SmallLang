Sleep :: (i32Duration) { Sleep_ext(i32Duration) } @export

extern ToString  :: () -> str
{ } @mscorlib.dll;System.Object;ToString

extern Sleep_ext :: (i32Duration)
{ } @mscorlib.dll;System.Threading.Thread;Sleep

extern Array_Resize<U> :: (&avntArray<U>, i32Size)
{ } @mscorlib.dll;System.Array;Resize

cast :: i16Parm -> strParm { return i16Parm.ToString() }

cast :: i32Parm -> strParm { return i32Parm.ToString() }

cast :: i64Parm -> strParm { return i64Parm.ToString() }

cast :: dblParm -> strParm { return dblParm.ToString() }

cast :: fltParm -> strParm { return fltParm.ToString() }

cast :: dtmParm -> strParm { return dtmParm.ToString() }

cast :: blnParm -> strParm { return blnParm.ToString() }


#-
	Basic data structures
-#

#Stack
struct Stack<T> -> stk
{
	avntNodes<T>
	i32Current
} @export

Push<T> :: (&stkStack<T>, vntItem<T>)
{
	stkStack.avntNodes[stkStack.i32Current] = vntItem
	stkStack.i32Current++
} @export

Pop<T> :: (&stkStack<T>) -> T
{
	stkStack.i32Current--
	return stkStack.avntNodes[stkStack.i32Current]
} @export

Current<T> :: (&stkStack<T>) -> T
{
	return stkStack.avntNodes[stkStack.i32Current - 1]
} @export

#List
Struct List<T> -> lst
{
	avntNodes<T>
	i32Current
	i32Capacity
	
	new 
	{
		avntNodes = [64]
		i32Capacity = 64
	}
} @export

Add<T> :: (&lstList<T>, vntItem<T>)
{
	if(lstList.i32Capacity <= lstList.i32Current)
	{
		lstList.i32Capacity *= 2
		Array_Resize(lstList.avntNodes, lstList.i32Capacity):T
	}
	
	lstList.avntNodes[lstList.i32Current] = vntItem	
	lstList.i32Current++
} @export

Item<T> :: (&lstList<T>, i32Index) -> T
{
	return lstList.avntNodes[i32Index]
} @export