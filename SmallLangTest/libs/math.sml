struct Vector2 -> v2
{
	i32X
	i32Y
} @export

struct Random -> rnd
{
	i32Z
	i32W
	
	new
	{
		i32Z = Time_ext()
		i32W = Time_ext()
	}
} @export

Min :: (i32Num1, i32Num2) -> i32Num
{
	if(i32Num1 < i32Num2) return i32Num1
	return i32Num2
} @export

Max :: (i32Num1, i32Num2) -> i32Num
{
	if(i32Num1 > i32Num2) return i32Num1
	return i32Num2
} @export

Clamp :: (i32Min, i32Max, i32Value) -> i32Num
{
	:i32Ret = i32Value
	if(i32Ret < i32Min) i32Ret = i32Min
	if(i32Ret > i32Max) i32Ret = i32Max
	return i32Ret
} @export

AddVector :: (v2Vec1, v2Vec2) -> v2
{
	:v2Ret = new
	v2Ret.i32X = v2Vec1.i32X + v2Vec2.i32X
	v2Ret.i32Y = v2Vec1.i32Y + v2Vec2.i32Y
	return v2Ret
} @export

Abs :: (i32Num) -> i32Num
{
	:i32Ret = i32Num
	if(i32Ret < 0) i32Ret *= -1
	return i32Ret
} @export

Random :: (&rndRandom, i32Min, i32Max) -> i32
{
	rndRandom.i32Z = 36969 * (rndRandom.i32Z and 65535) + (rndRandom.i32Z rshift 16)
	rndRandom.i32W = 18000 * (rndRandom.i32W and 65535) + (rndRandom.i32Z rshift 16)
	:i32Rand = Abs((rndRandom.i32Z lshift 16) + rndRandom.i32W)
	
	:dblRand = (cast(dbl) i32Rand + 1D) * (1 / (2 ^ 31) + 2)
	return cast(i32) (dblRand * ((i32Max - i32Min) + 1) + i32Min)
} @export

extern Time_ext :: () -> i32
{ } @mscorlib.dll;System.Environment;get_TickCount