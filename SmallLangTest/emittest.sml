import "libs\\stdlib"
import "libs\\graphics" as g
import "libs\\input" as i
import "libs\\math" as m

Execute :: ()
{	
	#:stkStack<str> = new
	#stkStack.avntNodes = [10]
	#Push(stkStack, "test"):Str
	#:strItem = Pop(stkStack):Str
	#g.PrintLine("Popped " ... strItem)
	
	:lstItems<str> = new
	lstItems.avntNodes = [20]
	lstItems.i32Capacity = 20
	Add(lstItems, "daPst"):Str
	Add(lstItems, "gaWbi"):Str
	Add(lstItems, "gdLrm"):Str
	Add(lstItems, "gaCel"):Str
	Add(lstItems, "eaFtl"):Str
	Add(lstItems, "xaPrd"):Str
	:rndRandom = new
	:strTest = " "
	:i32Index = m.Random(rndRandom, 0, lstItems.i32Current)
	g.PrintLine(Item(lstItems, i32Index):Str)
	
	:ndeNode = new
	g.PrintLine(**ndeNode.i32X ... " " ... **ndeNode.i32Y)
	
	i.Read()
} @run

struct MazeNode -> nde
{
	blnVisited
	i32X
	i32Y
	i32Walls
	
	new
	{
		i32X = 1
		i32Y = 1
	}
}