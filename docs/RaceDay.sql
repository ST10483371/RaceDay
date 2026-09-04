Create database RaceDay;
Use RaceDay;

CREATE TABLE Users(
UserID int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(50) NOT NULL,
Email VARCHAR(20) UNIQUE NOT NULL,
Password VARCHAR(20) NOT NULL,
ContactInfo VARCHAR(10),
UserType VARCHAR(50) NOT NULL,
CHECK (UserType IN ('Participant', 'Organizer'))
)

CREATE TABLE Participants(
ParticipantId int IDENTITY(1,1) PRIMARY KEY,
DateOfBirth VARCHAR(20) NOT NULL,
Gender VARCHAR(20) NOT NULL,
EmergencyContact VARCHAR(20) 
TeamName VARCHAR(20)
)

CREATE TABLE Organizers(
OrganizerID int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(20) UNIQUE NOT NULL,
Contact_Number(10) NOT NULL
)

CREATE TABLE Races(
RaceID int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(50) UNIQUE NOT NULL,
Location VARCHAR(50) NOT NULL,
StartTime VARCHAR(50) NOT NULL,
EndTime VARCHAR(50) NOT NULL,
Distance int NOT NULL,
Type VARCHAR(20) NOT NULL
)

CREATE TABLE Registrations(
RegistrationID int IDENTITY(1,1) PRIMARY KEY,
BibNumber int ,
Status VARCHAR(20),
RegistrationDate DATE,
ParticipantID int,
RaceID int 

FOREIGN KEY (ParticipantID)
REFERENCES Participants(ParticipantID),

FOREIGN KEY(RaceID)
REFERENCES Races(RaceID)
)

CREATE TABLE Categories(
CategoryID int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(20) NOT NULL,
Description VARCHAR(50) NOT NULL
)

CREATE TABLE RaceCategories(
CategoryID int,
RaceId int,

FOREIGN KEY (CategoryID)
REFERENCES Categories(CategoryID),

FOREIGN KEY (RaceID)
REFERENCES Races(RaceID)
)

CREATE TABLE Results(
ResultID int IDENTITY(1,1) PRIMARY KEY,
RegistrationID int,
FinishTime VARCHAR(50),
Position int,
Status VARCHAR(20),

FOREIGN KEY (RegistrationID)
REFERENCES Registrations(RegistrationID)
)




