#include <iostream>

thread_local int secret = 69;

extern "C"
{
	__declspec(dllexport) int __cdecl GetThreadLocalInt()
	{
		std::cout << "Address of secret: " << &secret << std::endl;
		return secret++;
	}
}
