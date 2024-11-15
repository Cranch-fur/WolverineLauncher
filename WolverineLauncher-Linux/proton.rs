use std::{env, error::Error, path::PathBuf, process::Command};

#[derive(Debug)]
pub struct ProtonVersion {
    pub name: String,
    pub path: PathBuf
}

fn use_wine() -> Result<Vec<ProtonVersion>, Box<dyn Error>> {
    println!("Couldn't find a Steam install. Using wine.");
    let winepath = Command::new("which").arg("wine").output()?;
    let winepath = String::from_utf8_lossy(&winepath.stdout).trim().to_string();
    let winepath = PathBuf::from(winepath);
    match &winepath.is_file() {
        true => Ok(vec![(ProtonVersion {
            name: "Wine".to_string(),
            path: winepath
        })]),
        false => Err("Could not find a Wine or Proton installation!".into())
    }
    
}

pub fn find_all() -> Result<Vec<ProtonVersion>, Box<dyn Error>> {
    (|| -> Result<Vec<ProtonVersion>, Box<dyn Error>> {
        let steam_dir = steamlocate::SteamDir::locate()?;
        let mut proton_versions: Vec<ProtonVersion> = Vec::new();
        for library in (steam_dir.libraries()?).flatten() {
            for app in library.apps().flatten() {
                let app_name = app.name.as_ref().unwrap();
                if app_name.contains("Proton") {
                    let app_path = library.resolve_app_dir(&app).join("proton");
                    if app_path.is_file() {
                        proton_versions
                            .push(
                                ProtonVersion {
                                    name: app_name.to_string(),
                                    path: app_path
                                }
                            );
                    } else {
                        eprintln!("{:?}", app_path);
                    }
                }
            }
        }
        if proton_versions.is_empty() {
            Err("No Proton versions found".into())
        } else {
            Ok(proton_versions)
        }
    })()
    .or_else(|_| use_wine())
}

pub fn find_highest(versions: &[ProtonVersion]) -> Option<&ProtonVersion> {
    versions.iter().max_by_key(|proton| {
        let version_parts: Vec<&str> = proton.name.split_whitespace().collect();
        if let Some(version) = version_parts.get(1) {
            match version.parse::<f64>() {
                Ok(n) => (n as i64, 0), // Numeric version
                Err(_) => {
                    if version.contains("Experimental") {
                        (2, 0) // Treat "Experimental" as Proton 2.0
                    } else if version.contains("Hotfix") {
                        (1, 0) // Treat "Hotfix" as Proton 1.0
                    } else {
                        (0, 0) // Non-numeric versions or special cases not handled above
                    }
                }
            }
        } else {
            (0, 0) // Default for non-parsable or missing version numbers
        }
    })
}

pub fn launch_exe(exe_path: &str, proton_path: &str) -> Result<(), Box<dyn Error>> {
    let display = env::var("DISPLAY").unwrap_or_else(|_| ":0".to_string());
    let prefix = PathBuf::from(env::var("HOME")?).join(".insomniac");
    if !prefix.is_dir() {
        let _ = std::fs::create_dir_all(&prefix);
    }

    if proton_path.to_lowercase().contains("wine") &&
        !proton_path.to_lowercase().contains("proton") {
            Command::new(proton_path)
            .env("WINEPREFIX", prefix)
            .env("WINEDLLOVERRIDES", "version.dll=n,b")
            .env("DISPLAY", display)
            .arg(exe_path)
            .spawn()?
            .wait()?;
        } else {
            Command::new(proton_path)
            .env("STEAM_COMPAT_CLIENT_INSTALL_PATH", steamlocate::SteamDir::locate()?.path())
            .env("STEAM_COMPAT_DATA_PATH", prefix)
            .env("WINEDLLOVERRIDES", "version.dll=n,b")
            .env("DISPLAY", display)
            .arg("run")
            .arg(exe_path)
            .spawn()?
            .wait()?;
        }
    Ok(println!("Launched {exe_path}"))
}
