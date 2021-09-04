#include <iostream>
#include "Secret.h"

thread_local int threadLocal = 0;

extern "C"
{
	__declspec(dllexport) __cdecl void Greet()
	{
		std::cout << "Hello World!" << std::endl;
		std::cout << "Here is my secret: " << GetSecret() << std::endl;
	}

	__declspec(dllexport) __cdecl int addNumbers(int a, int b)
	{
		std::cout << "Adding " << a << " and " << b << "..." << std::endl;
		return a + b;
	}

	__declspec(dllexport) __cdecl int GetThreadLocalInt()
	{
		std::cout << "Address: " << &threadLocal << std::endl;
		return threadLocal++;
	}
}