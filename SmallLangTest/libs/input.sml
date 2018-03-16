IsKeyDown :: (i32Key) -> blnDown { return IsKeyDown_ext(i32Key) } @export
ReadLine :: () -> str { return ReadLine_ext() } @export
Read :: () { ReadKey_ext() } @export
Sleep :: (i32Duration) { Sleep_ext(i32Duration) } @export

extern ReadKey_ext :: () -> obj
{ } @mscorlib.dll;System.Console;ReadKey

extern ReadLine_ext :: () -> str 
{ } @mscorlib.dll;System.Console;ReadLine

extern IsKeyDown_ext :: (i32Key) -> blnDown
{ } @PresentationCore.dll;System.Windows.Input.Keyboard;IsKeyDown

extern Sleep_ext :: (i32Duration)
{ } @mscorlib.dll;System.Threading.Thread;Sleep