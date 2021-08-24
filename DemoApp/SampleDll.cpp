#include <iostream>

extern "C"
{
	__declspec(dllexport) __cdecl void Greet()
	{
		std::cout << "Hello World!" << std::endl;
	}

	__declspec(dllexport) __cdecl int addNumbers(int a, int b)
	{
		std::cout << "Adding " << a << " and " << b << "..." << std::endl;
		return a + b;
	}
}