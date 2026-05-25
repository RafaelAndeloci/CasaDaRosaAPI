/*
    Script de bootstrap do primeiro usuário Admin.

    Pré-requisitos:
    1. A migration que adiciona a coluna Role em Users deve ter sido aplicada.
    2. Se a tabela Users ainda usar PhoneNumberRawValue como smallint, mantenha os campos de telefone como NULL.
    3. Ajuste os valores de nome, e-mail e senha hash se desejar outro usuário inicial.

    Senha em texto usada para gerar o hash abaixo:
    Admin@123456!

    IMPORTANTE:
    - Não armazene a senha em texto em produção.
    - Gere um novo hash antes de subir para ambientes reais.
*/

DECLARE @UserId UNIQUEIDENTIFIER = '7C22105A-614C-421C-AC26-374B2EC6256C';
DECLARE @CreatedAtUtc DATETIME2 = SYSUTCDATETIME();
DECLARE @PasswordHash NVARCHAR(500) = 'UWPow04lRmYSsKa9flqrdA==.Abp6p/afinvdun6/tkxdRy3Ieyn0wr4GynpFXgiA6yw=';

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'admin@casadarosa.local')
BEGIN
    INSERT INTO dbo.Users
    (
        Id,
        FirstName,
        Surname,
        Name,
        Email,
        PasswordHash,
        PhoneNumber,
        PhoneNumberRawValue,
        PhoneNumberAreaCode,
        PhoneNumberCountryCode,
        Role,
        Status,
        EmailConfirmationToken,
        EmailConfirmationTokenExpiresAtUtc,
        EmailConfirmedAtUtc,
        CreatedAtUtc,
        UpdatedAtUtc
    )
    VALUES
    (
        @UserId,
        'Admin',
        'Casa da Rosa',
        'Admin Casa da Rosa',
        'admin@casadarosa.local',
        @PasswordHash,
        NULL,
        NULL,
        NULL,
        NULL,
        2,
        0,
        '',
        '0001-01-01T00:00:00',
        @CreatedAtUtc,
        @CreatedAtUtc,
        NULL
    );
END
ELSE
BEGIN
    PRINT 'Usuário admin inicial já existe.';
END
