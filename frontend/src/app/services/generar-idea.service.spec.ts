import { TestBed } from '@angular/core/testing';

import { GenerarIdeaService } from './generar-idea.service';

describe('GenerarIdeaService', () => {
  let service: GenerarIdeaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GenerarIdeaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
