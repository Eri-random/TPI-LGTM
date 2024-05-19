import { TestBed } from '@angular/core/testing';

import { ReconocimientoTelaService } from './reconocimiento-tela.service';

describe('ReconocimientoTelaService', () => {
  let service: ReconocimientoTelaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ReconocimientoTelaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
