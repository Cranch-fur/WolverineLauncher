#include <iostream>
#include <filesystem>
#include <string>

using namespace std;
using namespace std::filesystem;








int main()
{
    bool windowModeSelected = false;
    string windowMode;
    while (windowModeSelected == false) {
        system("CLS");

        printf("SELECT WINDOW MODE TO RUN GAME WITH:\n");
        printf("[0] Windowed\n");
        printf("[1] Borderless Window\n");
        printf("> ");

        string userInput;
        bool isInt = true;
        cin >> userInput;

        for (char c : userInput) {
            if (!isdigit(c)) {
                isInt = false;
            }
        }

        if (isInt == false) {
            continue;
        }

        int userInputINT = atoi(userInput.c_str());
        if (userInputINT < 0 || userInputINT > 1) {
            continue;
        }

        switch (userInputINT)
        {
            case 0:
                windowMode = "";
                break;

            case 1:
                windowMode = " -borderlesswindow";
                break;
        }

        windowModeSelected = true;
    }


    path currentPath = current_path();
    currentPath /= "source";


    string commandLine = "\"Marvel's Wolverine.exe\" -source " + currentPath.string() + windowMode + " -fps_unlock_base -rumble -controllerspeaker -nopadremoved";
    system(commandLine.c_str());


    exit(0);
}