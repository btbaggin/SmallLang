import "..\\libs\\stdlib"
import "..\\libs\\graphics" as g
import "..\\libs\\input" as i
import "..\\libs\\math" as m

:ci32WIDTH = 50
:ci32HEIGHT = 14
:ci32UP_OPEN = 1
:ci32LEFT_OPEN = 2
:ci32DOWN_OPEN = 4
:ci32RIGHT_OPEN = 8
:cstrWALL = "#"

struct MazeNode -> nde
{
	blnVisited
	i32X
	i32Y
	i32Walls
}

struct GameState -> gs
{
	i32Width
	i32Height
	andeMaze
	m.v2CharPosition
}

Run ::()
{
	g.PrintLine("Welcome to my program")
	g.PrintLine("Press Enter to continue...")
	i.Read()
		
	:gsState = new
	gsState.i32Width = ci32WIDTH
	gsState.i32Height = ci32HEIGHT
	
	:m.rndRandom = new
	BuildMaze(gsState, rndRandom)
	
	#game loop
	:blnRunning = true
	while(!i.IsKeyDown(13))
	{
		g.ClearDisplay()
		PrintGrid(gsState)
		i.Read()
		gsState.v2CharPosition = GatherInput(gsState)
		
		if(gsState.v2CharPosition.i32X = ci32WIDTH - 1 & gsState.v2CharPosition.i32Y = ci32HEIGHT - 1)
			blnRunning = false
	}	
	
	g.ClearDisplay()
	g.PrintLine("You win!")
} @run

BuildMaze :: (&gsState, &m.rndRandom)
{
	:andeMaze = [ci32WIDTH * ci32HEIGHT]
	for(:i32Y = 0, i32Y < ci32HEIGHT, i32Y++)
	{
		for(:i32X = 0, i32X < ci32WIDTH, i32X++)
		{
			:i32Index = GetPosition(i32X, i32Y)
			
			andeMaze[i32Index].i32X = i32X
			andeMaze[i32Index].i32Y = i32Y
		}
	}
	
		
	:stkStack<nde> = new
	stkStack.avntNodes = [ci32WIDTH * ci32HEIGHT]
	
	ContinuePath(0, 0, andeMaze, rndRandom, stkStack)
	while (stkStack.i32Current > 0)
	{
		:ndeNode = Pop(stkStack):nde
		ContinuePath(ndeNode.i32X, ndeNode.i32Y, andeMaze, rndRandom, stkStack)
	}
	gsState.andeMaze = andeMaze
}

ContinuePath :: (i32X, i32Y, &andeMaze, &m.rndRandom, &stkStack<nde>)
{	
	:i32Index = GetPosition(i32X, i32Y)
	:ai32ValidNeighbors = [3]
	:ai32Direction = [3]
	:i32I = 0
	
	if(IsValid(i32X, i32Y - 1, andeMaze)) 
	{
		ai32ValidNeighbors[i32I] = GetPosition(i32X, i32Y - 1)
		ai32Direction[i32I] = ci32UP_OPEN
		i32I++
	}
	if(IsValid(i32X - 1, i32Y, andeMaze)) 
	{
		ai32ValidNeighbors[i32I] = GetPosition(i32X - 1, i32Y)
		ai32Direction[i32I] = ci32LEFT_OPEN
		i32I++
	}
	if(IsValid(i32X, i32Y + 1, andeMaze)) 
	{
		ai32ValidNeighbors[i32I] = GetPosition(i32X, i32Y + 1)
		ai32Direction[i32I] = ci32DOWN_OPEN
		i32I++
	}
	if(IsValid(i32X + 1, i32Y, andeMaze)) 
	{	
		ai32ValidNeighbors[i32I] = GetPosition(i32X + 1, i32Y)
		ai32Direction[i32I] = ci32RIGHT_OPEN
		i32I++
	}
	
	andeMaze[i32Index].blnVisited = true
	
	if (i32I > 0)
	{
		Push(stkStack, andeMaze[i32Index]):nde
		
		:i32Rand = m.Random(rndRandom, 0, i32I - 1)
		:i32NewIndex = ai32ValidNeighbors[i32Rand]		
		:i32Direction = ai32Direction[i32Rand]
	
		andeMaze[i32Index].i32Walls = andeMaze[i32Index].i32Walls or i32Direction
		andeMaze[i32Index] = andeMaze[i32Index]
		
		if (i32Direction = ci32DOWN_OPEN)
		{
			ContinuePath(i32X, i32Y + 1, andeMaze, rndRandom, stkStack)		
			andeMaze[i32NewIndex].i32Walls = andeMaze[i32NewIndex].i32Walls or ci32UP_OPEN		
		}
		else if (i32Direction = ci32LEFT_OPEN)
		{
			ContinuePath(i32X - 1, i32Y, andeMaze, rndRandom, stkStack)			
			andeMaze[i32NewIndex].i32Walls = andeMaze[i32NewIndex].i32Walls or ci32RIGHT_OPEN		
		}
		else if (i32Direction = ci32UP_OPEN)
		{
			ContinuePath(i32X, i32Y - 1, andeMaze, rndRandom, stkStack)
			andeMaze[i32NewIndex].i32Walls = andeMaze[i32NewIndex].i32Walls or ci32DOWN_OPEN			
		}
		else
		{	
			ContinuePath(i32X + 1, i32Y, andeMaze, rndRandom, stkStack)		
			andeMaze[i32NewIndex].i32Walls = andeMaze[i32NewIndex].i32Walls or ci32LEFT_OPEN		
		}	
	}
}

IsValid :: (i32X, i32Y, andeMaze) -> blnValid
{
	if(i32X < 0 | i32X >= ci32WIDTH | i32Y < 0 | i32Y >= ci32HEIGHT) return false
	
	:i32Index = GetPosition(i32X, i32Y)
	return !andeMaze[i32Index].blnVisited
}

GetPosition :: (i32X, i32Y) -> i32Index
{
	return i32X + (i32Y * ci32WIDTH)
}

PrintGrid :: (gsState)
{
	:andeMaze = gsState.andeMaze
	:strGrid = ""
	for(:i32Y = 0, i32Y < gsState.i32Height, i32Y++)
	{
		for(:i32I = 0, i32I < 2, i32I++)
		{
			for(:i32X = 0, i32X < gsState.i32Width, i32X++)
			{
				:i32Walls = andeMaze[GetPosition(i32X, i32Y)].i32Walls
				
				if(i32I = 0)
				{
					strGrid ..= cstrWALL
					if (i32Walls and ci32UP_OPEN = 0) strGrid ..= cstrWALL
					else strGrid ..= " "
				}
				else
				{
					if (i32Walls and ci32LEFT_OPEN = 0) strGrid ..= cstrWALL
					else strGrid ..= " "
					
					if (gsState.v2CharPosition.i32X = i32X & gsState.v2CharPosition.i32Y = i32Y) strGrid ..= "O"
					else strGrid ..= " "
				}
			}
			strGrid ..= cstrWALL
			strGrid ..= "\n"
		}
	}
	for(:i32X = 0, i32X < gsState.i32Width, i32X++)
	{
		strGrid ..= cstrWALL ... cstrWALL
	}
	strGrid ..= cstrWALL
	g.PrintLine(strGrid)
}

GatherInput :: (gsState) -> v2Position
{
	:m.v2Position = gsState.v2CharPosition
	
	:i32Index = GetPosition(v2Position.i32X, v2Position.i32Y)
	:i32Walls = gsState.andeMaze[i32Index].i32Walls
	
	if(i.IsKeyDown(25) & i32Walls and ci32RIGHT_OPEN != 0) v2Position.i32X++
	if(i.IsKeyDown(23) & i32Walls and ci32LEFT_OPEN != 0) v2Position.i32X--
	if(i.IsKeyDown(26) & i32Walls and ci32DOWN_OPEN != 0) v2Position.i32Y++
	if(i.IsKeyDown(24) & i32Walls and ci32UP_OPEN != 0) v2Position.i32Y--
	
	v2Position.i32X = m.Clamp(0, gsState.i32Width, v2Position.i32X)
	v2Position.i32Y = m.Clamp(0, gsState.i32Height, v2Position.i32Y)
	
	return v2Position
}

cast :: ndeNode -> str
{
	return **ndeNode.i32X ... " " ... **ndeNode.i32Y ... " " ... **ndeNode.blnVisited
}

