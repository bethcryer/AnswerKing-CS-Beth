output "ak_api_dotnet_ecs_instance" {
  value = aws_ecs_service.ecs_service.id
}
