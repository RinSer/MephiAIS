module TaigaTypes

    open System

    type TaigaProject = 
        { Id: int; 
        Name: string; 
        Description: string }

    type TaigaUser =
        { Id: int; 
        Username: string; 
        Full_name_display: string; 
        Gravatar_id: string  }

    type TaigaStatus = { Name: string }

    type TaigaTask =
        { Id: int;
        Project: int; 
        Assigned_to_extra_info: TaigaUser; 
        Created_date: DateTime;
        Due_date: Nullable<DateTime>;
        Subject: string;
        Status_extra_info: TaigaStatus}