mod proton;
use std::{env, error::Error, path::PathBuf};

fn main() {
    let proton_versions = match proton::find_all() {
        Ok(versions) => versions,
        Err(e) => {
            dialog_box::error(&e.to_string());
            panic!("{}", e);
        }
    };
    let proton = if proton_versions.first().unwrap().name == "Wine" {
        proton_versions.first().unwrap()
    } else {
        proton::find_highest(&proton_versions).unwrap()
    };
    match check_for_launcher() {
        Ok(launcher_path) => {
            match proton::launch_exe(
                launcher_path.to_str().expect("to_str failed on launcher_path"),
                proton.path.to_str().expect("to_str failed on proton_path")
            ) {
                Ok(_) => {}
                Err(e) => { dialog_box::error(&e.to_string()); }
            }
        },
        Err(e) => { dialog_box::error(&e.to_string()); }
    };
}

fn check_for_launcher() -> Result<PathBuf, Box<dyn Error>> {
    let mut path_to_launcher = env::current_dir()?;
    path_to_launcher = path_to_launcher.join("WolverineLauncher.exe");
    match path_to_launcher.exists() {
        true => Ok(path_to_launcher),
        false => Err("WolverineLauncher.exe could not be located!".into())
    }
}
