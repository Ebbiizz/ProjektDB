﻿CREATE TABLE Users (
    UserID INT NOT NULL PRIMARY KEY,
    Username VARCHAR(50) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL
);
