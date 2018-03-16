ClearDisplay :: () { Clear_ext() } @export
Print :: (strPrint) { Print_ext(strPrint) } @export
PrintLine :: () { PrintLine_ext("") } @export
PrintLine :: (strLine) { PrintLine_ext(strLine) } @export

extern Print_ext :: (strLine) 
{ } @mscorlib.dll;System.Console;Write

extern PrintLine_ext :: (strPrint) 
{ } @mscorlib.dll;System.Console;WriteLine

extern Clear_ext :: () 
{ } @mscorlib.dll;System.Console;Clear