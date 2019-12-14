fn main() {
    count_words(None);
}


fn count_words (optional:Option<String>) -> () {


    let mut input: String;


    //Only for tests
    match optional {
        Some(external_input) => input = external_input,
        None => {
            println!("Input Text");
            let string = util::let_user_input();
            input = string;
        }
    }


 

    println!("input = {}",input);
    let words_array:Vec<&str> = input.split(" ").collect();
    let mut counter:usize=0; //Zaehlt die Woerter in ""
    let mut in_quote = false;
    let mut compound_counter:usize = 0;
    for word in &words_array{
        if word != &"" || word != &"\"\"" {
            if  word.starts_with("\"") {
                in_quote = !in_quote;
                compound_counter+=1;
                counter+=1;
            }
            else if word.ends_with("\"") {
                in_quote = !in_quote;
                counter+=1;
            }
            else if in_quote {
                counter+=1;
            }  
        }
    }

    println!("all expressions: {}",words_array.len()-&counter+&compound_counter);
    println!("Compound Expressions {}",compound_counter);
        
}

#[test]
fn full_test() {
    count_words(Some("\"Voluptate ipsum dolor dolor incididunt\" laboris dolore laborum \"officia deserunt velit culpa qui non.".to_string()));
    count_words(Some("Amet amet enim \"Lorem consectetur\"".to_string()));
    count_words(Some("Id sint officia dolor in laboris do et labore adipisicing cillum irure nostrud.".to_string()));
    count_words(Some("\"Velit ex culpa minim ipsum officia.\"".to_string()));
    count_words(Some("Non elit occaecat et mollit enim.".to_string()));
    count_words(Some("Ullamco \"nostrud reprehenderit\" consequat quis dolore amet voluptate veniam. \"Cillum cillum ad aute excepteur amet commodo\" nostrud non elit non aliquip. \"Laborum cillum do sunt exercitation \"do amet non dolor sit ad consequat quis.".to_string()));
}
#[test]
fn small_test(){
    count_words(Some("Amet amet enim \"Lorem consectetur\"".to_string()));
}




use std::io::stdin;
#[allow(dead_code)]
pub fn let_user_input() -> String {
    let buffer = &mut String::new();
    stdin().read_line(buffer);
    let res = match buffer.trim_end(){
        "" => panic!("Please Input something"),
        input => input.to_owned(),
    };
    res
}
