SELECT customer.customername                       AS customer,
    TO_CHAR(appointment."yearMonth", 'Month YYYY') AS month,
    appointment.count                              AS appointments,
    participant.count                              AS participants,
    voting.count                                   AS votings
FROM "appointmentStatistic" appointment
         LEFT JOIN "participantStatistic" participant ON appointment."customerId" = participant."customerId"
         LEFT JOIN "votingStatistic" voting ON appointment."customerId" = voting."customerId"
         LEFT JOIN customer ON appointment."customerId" = customer.customerid
WHERE appointment."customerId" = '11111111-1111-1111-1111-111111111111'
ORDER BY appointment."yearMonth" DESC;